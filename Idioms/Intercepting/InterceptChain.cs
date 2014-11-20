using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Idioms.Depending;
using Decoratid.Idioms.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decoratid.Idioms.Intercepting
{
    public interface IInterceptChain<TArg, TResult>
    {
        List<InterceptLayer<TArg, TResult>> Layers { get; }
        LogicOfTo<TArg, TResult> FunctionToIntercept { get; }
    }

    /// <summary>
    /// defines a sequence of intercept layers that operate on some initial function TArg, TResult.  
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class InterceptChain<TArg, TResult> : IInterceptChain<TArg, TResult>
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public InterceptChain(Func<TArg, TResult> functionToIntercept)
        {
            this.Store = NaturalInMemoryStore.New();
            Condition.Requires(functionToIntercept).IsNotNull();
            this.FunctionToIntercept = functionToIntercept.MakeLogicOfTo();
        }
        #endregion

        
        #region Properties
        protected IStore Store { get;  set; }
        public LogicOfTo<TArg, TResult> FunctionToIntercept { get; private set; }
        public List<InterceptLayer<TArg, TResult>> Layers
        {
            get
            {
                return this.GetLayers();
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// an event that is fired when the intercept chain that is used by an InterceptUnitOfWork
        /// </summary>
        public event EventHandler<EventArgOf<InterceptUnitOfWork<TArg, TResult>>> Completed;
        #endregion

        #region Fluent Wire Methods
        public InterceptChain<TArg, TResult> AddIntercept(InterceptLayer<TArg, TResult> layer)
        {
            this.Store.SaveItemIfUniqueElseThrow(layer);
            return this;
        }
        public InterceptChain<TArg, TResult> AddIntercept(string id,
                Func<TArg, TArg> argDecorator,
                Action<TArg> argValidator,
                Func<TArg, TResult> action,
                Func<TResult, TResult> resultDecorator,
                Action<TResult> resultValidator)
        {
            InterceptLayer<TArg, TResult> layer = new InterceptLayer<TArg, TResult>(id, argDecorator, argValidator,
                action, resultDecorator, resultValidator);

            this.Store.SaveItemIfUniqueElseThrow(layer);
            return this;
        }
        public InterceptChain<TArg, TResult> SetDependency(string interceptId, string interceptItDependsOn)
        {
            var interceptFromStore = this.Store.Get<InterceptLayer<TArg, TResult>>(interceptId);
            if (interceptFromStore != null)
                throw new InvalidOperationException(string.Format("{0} is not registered", interceptId));

            interceptFromStore.AddDependency(interceptItDependsOn);
            this.Store.SaveItem(interceptFromStore);

            return this;
        }
        public InterceptChain<TArg, TResult> SetDependency(string interceptId, List<string> interceptsItDependsOn)
        {
            interceptsItDependsOn.WithEach(x =>
            {
                this.SetDependency(interceptId, x);
            });
            return this;
        }
        
        /// <summary>
        /// Adds an intercept around the last, currently most dependent intercept
        /// </summary>
        /// <param name="id"></param>
        /// <param name="argDecorator"></param>
        /// <param name="argValidator"></param>
        /// <param name="action"></param>
        /// <param name="resultDecorator"></param>
        /// <param name="resultValidator"></param>
        /// <returns></returns>
        public InterceptChain<TArg, TResult> AddNextIntercept(string id,
                Func<TArg, TArg> argDecorator,
                Action<TArg> argValidator,
                Func<TArg, TResult> action,
                Func<TResult, TResult> resultDecorator,
                Action<TResult> resultValidator)
        {
            
            var list = this.GetLayers();
            var last = list.LastOrDefault();
            if (last == null)
            {
                return this.AddIntercept(id, argDecorator, argValidator, action, resultDecorator, resultValidator);
            }
            else
            {
                InterceptLayer<TArg, TResult> layer = new InterceptLayer<TArg, TResult>(id, argDecorator, argValidator,
                    action, resultDecorator, resultValidator);
                layer.AddDependency(last.Dependency.Self);

                this.Store.SaveItemIfUniqueElseThrow(layer);
            }
            return this;
        }
        public InterceptChain<TArg, TResult> AddNextIntercept(InterceptLayer<TArg, TResult> layer)
        {
            var list = this.GetLayers();
            var last = list.LastOrDefault();
            if (last == null)
            {
                return this.AddIntercept(layer);
            }
            else
            {
                layer.AddDependency(last.Dependency.Self);
                this.Store.SaveItemIfUniqueElseThrow(layer);
            }
            return this;
        }
        #endregion
        
        #region Helpers
        private List<InterceptLayer<TArg, TResult>> GetLayers()
        {
            //get all the registered intercepts in order of dependency, least to most
            var list = this.Store.GetAllHasADependency<InterceptLayer<TArg, TResult>, string>();

            return list;
        }
        #endregion

        #region Methods
        /// <summary>
        /// performs the entire intercept process
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public TResult Perform(TArg arg, ILogger logger)
        {
            TResult returnValue = default(TResult);

            InterceptUnitOfWork<TArg, TResult> uow = new InterceptUnitOfWork<TArg, TResult>(this, arg, logger);

            try
            {
                returnValue = uow.Perform();
            }
            catch(Exception ex)
            {
                logger.LogError("InterceptChain Perform error", arg, ex);
                throw;
            }
            finally
            {
                //fire events
                this.Completed.BuildAndFireEventArgs(uow);
            }

            return returnValue;
        }
        #endregion
    }
}
