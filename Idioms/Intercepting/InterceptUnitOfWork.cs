using CuttingEdge.Conditions;
using Decoratid.Extensions;
using Decoratid.Idioms.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Core.Conditional;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Adjusting;
using Decoratid.Idioms.Observing;

namespace Decoratid.Idioms.Intercepting
{
    /// <summary>
    /// takes an interception chain and builds up a unit of work to perform.  
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class InterceptUnitOfWork<TArg, TResult> 
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public InterceptUnitOfWork(List<InterceptLayer<TArg, TResult>> layers, LogicOfTo<TArg, TResult> functionToIntercept, TArg arg)
        {
            Condition.Requires(layers).IsNotNull();
            Condition.Requires(functionToIntercept).IsNotNull();

            //if no logger is provided use an in memory store
            this.Logger = StoreLogger.New(NamedNaturalInMemoryStore.New("intercept log"));

            this.Layers = layers;
            this.FunctionToIntercept = functionToIntercept;
            this.Arg = arg;
        }
        #endregion

        #region IHasLogger
        public StoreLogger Logger { get; private set; }
        #endregion

        #region Properties
        public List<InterceptLayer<TArg, TResult>> Layers { get; private set; }
        public LogicOfTo<TArg, TResult> FunctionToIntercept { get; private set; }
        public TArg Arg { get; private set; }
        public TResult Result { get; private set; }
        public Exception Error { get; private set; }
        public IValueOf<TArg> DecoratedArg { get; private set; }
        public ILogic DecoratedLogic { get; private set; }
        public IValueOf<TResult> DecoratedResult { get; private set; }
        public TArg ProcessedArg { get; private set; }
        public TResult ProcessedResult { get; private set; }
        #endregion

        #region Calculated Properties
        public List<string> LogEntries
        {
            get
            {
                return (this.Logger as StoreLogger).LogEntries;
            }
        }
        #endregion

        #region Methods
        public TResult Perform()
        {
            this.Logger.Do((x) => x.LogVerbose("Unit of work started", null));
            try
            {
                //run thru the steps
                this.BuildArgDecoration();
                this.BuildLogicDecoration();
                this.InvokeDecoratedArgAndLogic();
            }
            catch (Exception ex)
            {
                this.Error = ex;
                this.Logger.Do((x) => x.LogError("Unit of work error", null, ex));
            }
            finally
            {
                this.Logger.Do((x) => x.LogVerbose("Unit of work ended", null));
            }
            return this.ProcessedResult;
        }
        /// <summary>
        /// builds up the arg as a ValueOf with a bunch of adjustments and observers
        /// </summary>
        private void BuildArgDecoration()
        {
            this.Logger.Do((x) => x.LogVerbose("BuildArgDecoration started", null));
            var intercepts = this.Layers;

            //decorate the argument
            IValueOf<TArg> argOf = null;
            if (this.Arg != null)
            {
                argOf = this.Arg.AsNaturalValue();

                //decorate the adjustments
                intercepts.WithEach((intercept) =>
                {
                    if (intercept.ArgDecorator != null)
                    {
                        argOf = argOf.Adjust(intercept.ArgDecorator);
                    }
                });
                //decorate the observations
                intercepts.WithEach((intercept) =>
                {
                    if (intercept.ArgValidator != null)
                    {
                        argOf = argOf.Observe(null, LogicOf<IValueOf<TArg>>.New((x) =>
                        {
                            intercept.ArgValidator.Perform(argOf);
                        }));
                    }
                });
                this.DecoratedArg = argOf;
            }
            this.Logger.Do((x) => x.LogVerbose("BuildArgDecoration completed", null));
        }
        /// <summary>
        /// builds up the logic as an ILogic with a bunch of adjustments 
        /// </summary>
        private void BuildLogicDecoration()
        {
            this.Logger.Do((x) => x.LogVerbose("BuildLogicDecoration started", null));

            var intercepts = this.Layers;

            //decorate the function
            ILogic logic = this.FunctionToIntercept;
            intercepts.WithEach((intercept) =>
            {
                if (intercept.Action != null)
                {
                    logic = logic.Adjust(LogicOfTo<ILogic, ILogic>.New((x) =>
                    {
                        return intercept.Action;
                    }));
                }
            });
            this.DecoratedLogic = logic;

            this.Logger.Do((x) => x.LogVerbose("BuildLogicDecoration completed", null));
        }
        /// <summary>
        /// invokes the arg decorations, then the logic decorations, then invokes the logic
        /// </summary>
        private TResult InvokeDecoratedArgAndLogic()
        {
            this.Logger.Do((x) => x.LogVerbose("InvokeDecoratedArgAndLogic started", null));

            this.Logger.Do((x) => x.LogVerbose("Arg", this.Arg));
            this.ProcessedArg = this.DecoratedArg.GetValue(); //invoke arg decoration chain (adjusters and observers)
            this.Logger.Do((x) => x.LogVerbose("ProcessedArg", this.ProcessedArg));

            ILogicOf<TArg> logicOf = (ILogicOf<TArg>)this.DecoratedLogic;
            var logicResults = logicOf.Perform(this.ProcessedArg) as LogicOfTo<TArg, TResult> ;
            this.Logger.Do((x) => x.LogVerbose("Logic performed", null));
            this.Result = logicResults.Result;
            this.Logger.Do((x) => x.LogVerbose("Result", this.Result));

            //decorate the result
            this.Logger.Do((x) => x.LogVerbose("Decorate result started", null));
            var intercepts = this.Layers;

            if (this.Result != null)
            {
                IValueOf<TResult> resultOf = this.Result.AsNaturalValue();

                //decorate the adjustments
                intercepts.WithEach((intercept) =>
                {
                    if (intercept.ResultDecorator != null)
                    {
                        resultOf = resultOf.Adjust(intercept.ResultDecorator);
                    }
                });
                //decorate the observations
                intercepts.WithEach((intercept) =>
                {
                    if (intercept.ResultValidator != null)
                    {
                        resultOf = resultOf.Observe(null, LogicOf<IValueOf<TResult>>.New((x) =>
                        {
                            intercept.ResultValidator.Perform(resultOf);
                        }));
                    }
                });
                this.Logger.Do((x) => x.LogVerbose("Decorate result completed", null));

                this.DecoratedResult = resultOf;
                this.ProcessedResult = resultOf.GetValue(); //invoke the decorations

                this.Logger.Do((x) => x.LogVerbose("ProcessedResult", this.ProcessedResult));
            }

            return this.ProcessedResult;
        }

        #endregion


    }
}
