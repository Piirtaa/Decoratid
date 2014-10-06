using CuttingEdge.Conditions;
using Decoratid.Extensions;
using Decoratid.Idioms.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decoratid.Idioms.Intercepting
{
    /// <summary>
    /// contains the state of an operation that has run thru an intercept chain.
    /// Mutators duplicate the sig of the intercept, but wrap with exception handling
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class InterceptUnitOfWork<TArg, TResult> : IHasLogger
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public InterceptUnitOfWork(InterceptChain<TArg, TResult> interceptChain, TArg arg, ILogger logger)
        {
            Condition.Requires(interceptChain).IsNotNull();
            this.Layers = interceptChain.GetLayers();
            this.Arg = arg;

            this.Logger = logger;
        }
        #endregion

        #region IHasLogger
        public ILogger Logger { get; private set; }
        #endregion

        #region Properties
        private List<InterceptLayer<TArg, TResult>> Layers { get; set; }
        public TArg Arg { get; private set; }
        public TArg DecoratedArg { get; private set; }
        public TResult Result { get; private set; }
        public TResult DecoratedResult { get; private set; }
        public Exception Error { get; private set; }
        #endregion

        #region Methods
        public TResult Perform()
        {
            try
            {
                //run thru the steps
                this.DecorateArgument();
                this.ValidateArgument();

                //go to the outermost decoration and execute its do
                var last = this.Layers.Last();
                this.Result = last.Do(this.DecoratedArg);

                this.DecorateResult();
                this.ValidateResult();
            }
            catch (Exception ex)
            {
                this.Error = ex;
                this.Logger.Do((x) => x.LogError("Unit of work error", this));

                throw;
            }

            return this.DecoratedResult;
        }
        #endregion

        #region Step Methods
        /// <summary>
        /// walks dependencies from least to most and decorates the arg
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected TArg DecorateArgument()
        {
            this.Logger.Do((x) => x.LogDebug("Starting DecorateArgument", this.Arg));

            //decorate the argument
            TArg decArg = this.Arg;
            this.Layers.WithEach(layer =>
            {
                try
                {
                    decArg = layer.DecorateArg(decArg);
                    
                    //track decoration progress
                    this.DecoratedArg = decArg;

                    this.Logger.Do((x) => x.LogDebug(string.Format("DecorateArgument on layer {0}", layer.Id), this.DecoratedArg));
                }
                catch (Exception ex)
                {
                    this.Logger.Do((x) => x.LogError(string.Format("DecorateArgument on layer {0}", layer.Id), this.DecoratedArg, ex));
                
                    throw new ArgDecorationInterceptionException<TArg, TResult>(layer, this, ex.Message, ex);
                }
            });
            this.DecoratedArg = decArg;
            return this.DecoratedArg;
        }

        /// <summary>
        /// walks dependencies from least tomost and validates
        /// </summary>
        /// <param name="arg"></param>
        protected void ValidateArgument()
        {
            this.Logger.Do((x) => x.LogDebug("Starting ValidateArgument", this.DecoratedArg));

            this.Layers.WithEach(layer =>
            {
                try
                {
                    layer.ValidateArg(this.DecoratedArg);
                    this.Logger.Do((x) => x.LogDebug(string.Format("ValidateArgument on layer {0}", layer.Id), this.DecoratedArg));
                }
                catch (Exception ex)
                {
                    this.Logger.Do((x) => x.LogError(string.Format("ValidateArgument on layer {0}", layer.Id), this.DecoratedArg, ex));
                    throw new ArgValidationInterceptionException<TArg, TResult>(layer, this, ex.Message, ex);
                }
            });
        }

        protected TResult DecorateResult()
        {
            this.Logger.Do((x) => x.LogDebug("Starting DecorateResult", this.Result));

            //decorate 
            TResult decRes = this.Result;
            this.Layers.WithEach(layer =>
            {
                try
                {
                    decRes = layer.DecorateResult(decRes);

                    //track decoration progress
                    this.DecoratedResult = decRes;
                    
                    this.Logger.Do((x) => x.LogDebug(string.Format("DecorateResult on layer {0}", layer.Id), this.DecoratedResult));
                }
                catch (Exception ex)
                {
                    this.Logger.Do((x) => x.LogError(string.Format("DecorateResult on layer {0}", layer.Id), this.DecoratedResult, ex));
                    throw new ResultDecorationInterceptionException<TArg, TResult>(layer, this, ex.Message, ex);
                }
            });

            return this.DecoratedResult;
        }

        /// <summary>
        /// walks dependencies from least tomost and validates
        /// </summary>
        /// <param name="arg"></param>
        protected void ValidateResult()
        {
            this.Logger.Do((x) => x.LogDebug("Starting ValidateResult", this.DecoratedResult));

            this.Layers.WithEach(layer =>
            {
                try
                {
                    layer.ValidateResult(this.DecoratedResult);

                    this.Logger.Do((x) => x.LogDebug(string.Format("ValidateResult on layer {0}", layer.Id), this.DecoratedResult));
                }
                catch (Exception ex)
                {
                    this.Logger.Do((x) => x.LogError(string.Format("ValidateResult on layer {0}", layer.Id), this.DecoratedResult, ex));
                    throw new ResultValidationInterceptionException<TArg, TResult>(layer, this, ex.Message, ex);
                }
            });
        }
        #endregion
    }
}
