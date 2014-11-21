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
            Condition.Requires(layers).IsNotNull().IsNotEmpty();
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
                this.DecorateArg();
                this.DecorateLogic();
                this.PerformDecorated();
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
        private void DecorateArg()
        {
            this.Logger.Do((x) => x.LogVerbose("DecorateArg started", null));
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
                            intercept.ArgValidator.CloneAndPerform(argOf);
                        }));
                    }
                });
                this.DecoratedArg = argOf;
            }
            this.Logger.Do((x) => x.LogVerbose("DecorateArg completed", null));
        }
        /// <summary>
        /// builds up the logic as an ILogic with a bunch of adjustments 
        /// </summary>
        private void DecorateLogic()
        {
            this.Logger.Do((x) => x.LogVerbose("DecorateLogic started", null));

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

            this.Logger.Do((x) => x.LogVerbose("DecorateLogic completed", null));
        }
        /// <summary>
        /// invokes the arg decorations, then the logic decorations, then invokes the logic
        /// </summary>
        private TResult PerformDecorated()
        {
            this.Logger.Do((x) => x.LogVerbose("PerformDecorated started", null));

            this.Logger.Do((x) => x.LogVerbose("Arg", this.Arg));
            this.ProcessedArg = this.DecoratedArg.GetValue(); //invoke arg decoration chain
            this.Logger.Do((x) => x.LogVerbose("ProcessedArg", this.ProcessedArg));

            ILogicOf<TArg> logicOf = (ILogicOf<TArg>)this.DecoratedLogic;
            logicOf.Context = this.ProcessedArg.AsNaturalValue();
            this.Logger.Do((x) => x.LogVerbose("Logic context set", null));

            ILogicTo<TResult> logicTo = (ILogicTo<TResult>)this.DecoratedLogic;
            logicTo.Perform();
            this.Logger.Do((x) => x.LogVerbose("Logic performed", null));

            this.Result = logicTo.Result;
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
                            intercept.ResultValidator.CloneAndPerform(resultOf);
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
