using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Core.ValueOfing;
using Decoratid.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decoratid.Idioms.Intercepting.Decorating
{
    /// <summary>
    /// defines a sequence of decorating intercept layers that operate on some initial function TArg, TResult.  
    /// </summary>
    /// <remarks>
    public class DecoratingInterceptChain<TArg, TResult> 
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public DecoratingInterceptChain(Func<TArg, TResult> functionToIntercept)
        {
            this.StoreOfIntercepts = new NaturalInMemoryStore();
            Condition.Requires(functionToIntercept).IsNotNull();
            this.FunctionToIntercept = functionToIntercept.MakeLogicOfTo();
        }
        #endregion

        #region Properties
        protected IStore StoreOfIntercepts { get; set; }
        protected LogicOfTo<TArg, TResult> FunctionToIntercept { get; set; }
        #endregion
  
        #region Events
        /// <summary>
        /// an event that is fired when the intercept chain is used by an InterceptUnitOfWork
        /// </summary>
        public event EventHandler<EventArgOf<DecoratingInterceptUnitOfWork<TArg, TResult>>> Completed;
        #endregion

        #region Fluent Wire Methods
        public DecoratingInterceptChain<TArg, TResult> AddIntercept(InterceptLayer<TArg, TResult> layer)
        {
            this.StoreOfIntercepts.SaveItemIfUniqueElseThrow(layer);
            return this;
        }
        public DecoratingInterceptChain<TArg, TResult> AddIntercept(string id,
                LogicOfTo<Onion<TArg>, OnionLayer<TArg>> argDecorator,
                LogicOf<Onion<TArg>> argValidator,
                LogicOfTo<Onion<TResult>, OnionLayer<TResult>> resultDecorator,
                LogicOf<Onion<TResult>> resultValidator)
        {
            DecoratingInterceptLayer<TArg, TResult> layer = new DecoratingInterceptLayer<TArg, TResult>(id, argDecorator, argValidator,
                 resultDecorator, resultValidator);

            this.StoreOfIntercepts.SaveItemIfUniqueElseThrow(layer);
            return this;
        }
        public DecoratingInterceptChain<TArg, TResult> SetDependency(string interceptId, string interceptItDependsOn)
        {
            var interceptFromStore = this.StoreOfIntercepts.Get<DecoratingInterceptLayer<TArg, TResult>>(interceptId);
            if (interceptFromStore != null)
                throw new InvalidOperationException(string.Format("{0} is not registered", interceptId));

            //var interceptItDependsOnFromStore = this.Store.GetById<InterceptLayer<TArg, TResult>>(interceptItDependsOn);
            //if (interceptItDependsOnFromStore != null)
            //    throw new InvalidOperationException(string.Format("{0} is not registered", interceptItDependsOn));

            interceptFromStore.AddDependency(interceptItDependsOn);
            this.StoreOfIntercepts.SaveItem(interceptFromStore);

            return this;
        }
        public DecoratingInterceptChain<TArg, TResult> SetDependency(string interceptId, List<string> interceptsItDependsOn)
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
        public DecoratingInterceptChain<TArg, TResult> AddNextIntercept(string id,
                LogicOfTo<Onion<TArg>, OnionLayer<TArg>> argDecorator,
                LogicOf<Onion<TArg>> argValidator,
                LogicOfTo<Onion<TResult>, OnionLayer<TResult>> resultDecorator,
                LogicOf<Onion<TResult>> resultValidator)
        {

            var list = this.GetLayers();
            var last = list.LastOrDefault();

            DecoratingInterceptLayer<TArg, TResult> layer = new DecoratingInterceptLayer<TArg, TResult>(id, argDecorator, argValidator,
                 resultDecorator, resultValidator);

            if (last != null)
            {
                layer.AddDependency(last.Dependency.Self);
            }

            this.StoreOfIntercepts.SaveItemIfUniqueElseThrow(layer);
            return this;
        }
        public DecoratingInterceptChain<TArg, TResult> AddNextIntercept(DecoratingInterceptLayer<TArg, TResult> layer)
        {
            var list = this.GetLayers();
            var last = list.Last();

            if (last != null)
            {
                layer.AddDependency(last.Dependency.Self);
            }

            this.StoreOfIntercepts.SaveItemIfUniqueElseThrow(layer);
            return this;
        }
        #endregion

        #region Helpers
        public DecoratingInterceptLayer<TArg, TResult> WithLayer(string id)
        {
            return this.StoreOfIntercepts.Get<DecoratingInterceptLayer<TArg, TResult>>(id);
        }
        public List<DecoratingInterceptLayer<TArg, TResult>> GetLayers()
        {
            //get all the registered intercepts in order of dependency, least to most
            var list = this.StoreOfIntercepts.GetAllHasADependency<DecoratingInterceptLayer<TArg, TResult>, string>();

            return list;
        }
        #endregion

        #region Methods
        /// <summary>
        /// performs the entire intercept process
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public TResult Perform(IValueOf<TArg> arg)
        {
            Onion<TResult> returnValue = null;

            DecoratingInterceptUnitOfWork<TArg, TResult> uow = new DecoratingInterceptUnitOfWork<TArg, TResult>(this.GetLayers(),
                this.FunctionToIntercept, arg);

            try
            {
                returnValue = uow.Perform();
            }
            catch
            {
                throw;
            }
            finally
            {
                //fire events
                this.Completed.BuildAndFireEventArgs(uow);
            }

            return returnValue.LastValue;
        }
        #endregion
    }
}
