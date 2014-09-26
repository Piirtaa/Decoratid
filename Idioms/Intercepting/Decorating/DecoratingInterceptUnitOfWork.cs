using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.Storing.Products;
using Decoratid.Extensions;
using CuttingEdge.Conditions;
using Decoratid.Thingness;
using Decoratid.Logging;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;

namespace Decoratid.Idioms.Intercepting.Decorating
{
    /// <summary>
    /// contains the state of an operation that has run thru an intercept chain.
    /// Mutators duplicate the sig of the intercept, but wrap with exception handling
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class DecoratingInterceptUnitOfWork<TArg, TResult> 
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public DecoratingInterceptUnitOfWork(List<DecoratingInterceptLayer<TArg, TResult>> layers,
            LogicOfTo<TArg, TResult> coreFunction, 
            IValueOf<TArg> arg)
        {
            Condition.Requires(layers).IsNotNull();
            Condition.Requires(coreFunction).IsNotNull();

            this.Layers = layers;
            this.Arg = new Onion<TArg>(arg.GetValue()); //build the first decoration layer
            this.CoreFunction = coreFunction;
            this._logger = LoggingManager.Instance.GetLogger();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Logger injected on ctor.  To tweak perf of this we should broker different loggers (or null)
        /// </summary>
        private ILogger _logger { get; set; }
        private List<DecoratingInterceptLayer<TArg, TResult>> Layers { get; set; }
        public LogicOfTo<TArg, TResult> CoreFunction { get; private set; }
        public Onion<TArg> Arg { get; private set; }
        public Onion<TResult> Result { get; private set; }
        public Exception Error { get; private set; }
        #endregion

        #region Methods
        public Onion<TResult> Perform()
        {
            try
            {
                //run thru the steps
                this.DecorateArgument();
                this.ValidateArgument();

                //execute the core function with the last core value
                var result = this.CoreFunction.CloneAndPerform(this.Arg.LastValue.ValueOf());
                this.Result = new Onion<TResult>(result);

                this.DecorateResult();
                this.ValidateResult();
            }
            catch (Exception ex)
            {
                this.Error = ex;
                this._logger.Do((x) => x.LogError("Unit of work error", this));

                throw;
            }

            return this.Result;
        }
        #endregion

        #region Step Methods
        /// <summary>
        /// walks dependencies from least to most and decorates the arg
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected Onion<TArg> DecorateArgument()
        {
            this._logger.Do((x) => x.LogDebug("Starting DecorateArgument", this.Arg));

            this.Layers.WithEach(layer =>
            {
                try
                {
                    if (layer.ArgDecorator == null)
                    {
                        //add a blank layer
                        this.Arg.AddUnmodifiedLayer(null, layer.Id);
                    }
                    else
                    {
                        //build the next layer
                        var valOfArg = this.Arg.ValueOf();
                        var onionLayer = layer.ArgDecorator.CloneAndPerform(valOfArg);
                        this.Arg.AddLayer(onionLayer);
                    }

                    this._logger.Do((x) => x.LogDebug(string.Format("DecorateArgument on layer {0}", layer.Id), this.Arg));
                }
                catch (Exception ex)
                {
                    this._logger.Do((x) => x.LogError(string.Format("DecorateArgument on layer {0}", layer.Id), this.Arg, ex));
                
                    throw new ArgDecorationDecoratingInterceptionException<TArg, TResult>(layer, this, ex.Message, ex);
                }
            });
            return this.Arg;
        }

        /// <summary>
        /// walks dependencies from least tomost and validates
        /// </summary>
        /// <param name="arg"></param>
        protected void ValidateArgument()
        {
            this._logger.Do((x) => x.LogDebug("Starting ValidateArgument", this.Arg));

            this.Layers.WithEach(layer =>
            {
                try
                {
                    if (layer.ArgValidator != null)
                        layer.ArgValidator.CloneAndPerform(this.Arg.ValueOf());

                    this._logger.Do((x) => x.LogDebug(string.Format("ValidateArgument on layer {0}", layer.Id), this.Arg));
                }
                catch (Exception ex)
                {
                    this._logger.Do((x) => x.LogError(string.Format("ValidateArgument on layer {0}", layer.Id), this.Arg, ex));
                    throw new ArgValidationDecoratingInterceptionException<TArg, TResult>(layer, this, ex.Message, ex);
                }
            });
        }

        protected Onion<TResult> DecorateResult()
        {
            this._logger.Do((x) => x.LogDebug("Starting DecorateResult", this.Result));

            //decorate 
            this.Layers.WithEach(layer =>
            {
                try
                {
                    if (layer.ResultDecorator == null)
                    {
                        //add a blank layer
                        this.Result.AddUnmodifiedLayer(null, layer.Id);
                    }
                    else
                    {
                        //build the next layer
                        var valOfResult = this.Result.ValueOf();
                        var onionLayer = layer.ResultDecorator.CloneAndPerform(valOfResult);
                        this.Result.AddLayer(onionLayer);
                    }
                   
                    this._logger.Do((x) => x.LogDebug(string.Format("DecorateResult on layer {0}", layer.Id), this.Result));
                }
                catch (Exception ex)
                {
                    this._logger.Do((x) => x.LogError(string.Format("DecorateResult on layer {0}", layer.Id), this.Result, ex));
                    throw new ResultDecorationDecoratingInterceptionException<TArg, TResult>(layer, this, ex.Message, ex);
                }
            });

            return this.Result;
        }

        /// <summary>
        /// walks dependencies from least tomost and validates
        /// </summary>
        /// <param name="arg"></param>
        protected void ValidateResult()
        {
            this._logger.Do((x) => x.LogDebug("Starting ValidateResult", this.Result));

            this.Layers.WithEach(layer =>
            {
                try
                {
                    if (layer.ResultValidator != null)
                        layer.ResultValidator.CloneAndPerform(this.Result.ValueOf());

                    this._logger.Do((x) => x.LogDebug(string.Format("ValidateResult on layer {0}", layer.Id), this.Result));
                }
                catch (Exception ex)
                {
                    this._logger.Do((x) => x.LogError(string.Format("ValidateResult on layer {0}", layer.Id), this.Result, ex));
                    throw new ResultValidationDecoratingInterceptionException<TArg, TResult>(layer, this, ex.Message, ex);
                }
            });
        }
        #endregion
    }
}
