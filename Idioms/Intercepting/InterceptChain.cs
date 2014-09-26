using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Storing;
using Decoratid.Idioms.Storing.Products;
using Decoratid.Extensions;
using Decoratid.Idioms.Storing.Core;
using System.Runtime.Serialization;
using Decoratid.Core.Logical;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Idioms.Intercepting
{
    /// <summary>
    /// defines a sequence of intercept layers that operate on some initial function TArg, TResult.  
    /// </summary>
    /// <remarks>
    /// Within a chain of Intercepts, the execution is as follows:
    /// 1.  from least dependant intercept to most -> decorate argument.  using the clothing metaphor, we want the last intercepting,
    /// layer (eg. the most dependent) to, ultimately, dress the argument how it wants.
    /// 2.  from least dependant intercept to most -> validate argument.  likewise, the validations should pass by each layer, with the
    /// last layer ultimately deciding what is valid, after each layer's check.
    /// 3.  from most dependant intercept to least -> if there is an action defined, perform it.  traverse dependencies until an action is defined.
    /// Note:  to duplicate the behaviour of "inheritance", each layer has access to a "BaseLayer" property that wires to the next less-dependent layer.
    /// The clothing metaphor - one behaves as they are dressed.
    /// 4.  from least dependant intercept to most -> decorate return value.  clothing metaphor applies here as with steps 1 and 2.
    /// 5.  from least dependant intercept to most -> validate return value.  clothing metaphor, again.
    /// 
    /// We want layers to be able to ensure the contractual requirements to their execution, such that the layers cannot be bypassed.
    /// 
    /// </remarks>
    public class InterceptChain<TArg, TResult> 
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public InterceptChain(Func<TArg, TResult> functionToIntercept)
        {
            this.Store = new NaturalInMemoryStore();
            Condition.Requires(functionToIntercept).IsNotNull();
            this.FunctionToIntercept = functionToIntercept.MakeLogicOfTo();
        }
        #endregion

        
        #region Properties
        protected IStore Store { get;  set; }
        protected LogicOfTo<TArg, TResult> FunctionToIntercept { get; set; }
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

            //var interceptItDependsOnFromStore = this.Store.GetById<InterceptLayer<TArg, TResult>>(interceptItDependsOn);
            //if (interceptItDependsOnFromStore != null)
            //    throw new InvalidOperationException(string.Format("{0} is not registered", interceptItDependsOn));

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
            var last = list.Last();

            InterceptLayer<TArg, TResult> layer = new InterceptLayer<TArg, TResult>(id, argDecorator, argValidator,
                action, resultDecorator, resultValidator);
            layer.AddDependency(last.Dependency.Self);

            this.Store.SaveItemIfUniqueElseThrow(layer);
            return this;
        }
        public InterceptChain<TArg, TResult> AddNextIntercept(InterceptLayer<TArg, TResult> layer)
        {
            var list = this.GetLayers();
            var last = list.Last();

            layer.AddDependency(last.Dependency.Self);

            this.Store.SaveItemIfUniqueElseThrow(layer);
            return this;
        }
        #endregion
        
        #region Helpers
        public List<InterceptLayer<TArg, TResult>> GetLayers()
        {
            //get all the registered intercepts in order of dependency, least to most
            var list = this.Store.GetAllHasADependency<InterceptLayer<TArg, TResult>, string>();

            //set the initial layer
            var initialLayer = new InterceptLayer<TArg, TResult>("initial");
            initialLayer.SetAction(this.FunctionToIntercept);

            //wire the layers together
            InterceptLayer<TArg, TResult> lastLayer = initialLayer; //the first layer is the function being intercepted
            list.WithEach(layer =>
            {
                layer.BaseLayer = lastLayer;
                lastLayer = layer;
            });

            return list;
        }

        #endregion

        #region Methods
        /// <summary>
        /// performs the entire intercept process
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public TResult Perform(TArg arg)
        {
            TResult returnValue = default(TResult);

            InterceptUnitOfWork<TArg, TResult> uow = new InterceptUnitOfWork<TArg, TResult>(this, arg);

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

            return returnValue;
        }
        #endregion
    }
}
