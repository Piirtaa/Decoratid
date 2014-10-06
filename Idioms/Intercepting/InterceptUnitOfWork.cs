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
    /// takes an interception chain and converts this into a set of decorations 
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

            this.Logger = logger;
            this.InterceptChain = interceptChain;
            this.Arg = arg;
        }
        #endregion

        #region IHasLogger
        public ILogger Logger { get; private set; }
        #endregion

        #region Properties   
        public InterceptChain<TArg, TResult> InterceptChain { get; private set; }
        public TArg Arg { get; private set; }
        public TResult Result { get; private set; }
        public Exception Error { get; private set; }
        IValueOf<TArg> DecoratedArg { get; set; }
        ILogic DecoratedLogic { get; set; }
        IValueOf<TResult> DecoratedResult { get; set; }
        #endregion

        #region Methods
        public TResult Perform()
        {
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
                this.Logger.Do((x) => x.LogError("Unit of work error", this));

                throw;
            }

            return this.DecoratedResult.GetValue();
        }
        private void DecorateArg()
        {
            this.Logger.Do((x) => x.LogVerbose("DecorateArg started", null));
            var intercepts = this.InterceptChain.Layers;

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
        private void DecorateLogic()
        {
            this.Logger.Do((x) => x.LogVerbose("DecorateLogic started", null));

            var intercepts = this.InterceptChain.Layers;

            //decorate the function
            ILogic logic = this.InterceptChain.FunctionToIntercept;
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
        private void PerformDecorated()
        {
            this.Logger.Do((x) => x.LogVerbose("PerformDecorated started", null));

            this.Logger.Do((x) => x.LogVerbose("Arg", this.Arg));
            var arg = this.DecoratedArg.GetValue(); //invoke arg decoration chain
            this.Logger.Do((x) => x.LogVerbose("DecoratedArg", arg)); 
            
            ILogicOf<TArg> logicOf = (ILogicOf<TArg>)this.DecoratedLogic;
            logicOf.Context = arg.AsNaturalValue();
            this.Logger.Do((x) => x.LogVerbose("Logic context set", null));
            
            ILogicTo<TResult> logicTo = (ILogicTo<TResult>)this.DecoratedLogic;
            logicTo.Perform();
            this.Logger.Do((x) => x.LogVerbose("Logic performed", null));

            var result = logicTo.Result;
            this.Result = result;
            this.Logger.Do((x) => x.LogVerbose("Result", result));
            
            //decorate the result
            this.Logger.Do((x) => x.LogVerbose("Decorate result started", null));
            var intercepts = this.InterceptChain.Layers;
            
            if (result != null)
            {
                IValueOf<TResult> resultOf = result.AsNaturalValue();
                
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
                var decRes = resultOf.GetValue();

                this.Logger.Do((x) => x.LogVerbose("DecoratedResult", decRes));
            }

        }

        #endregion


    }
}
