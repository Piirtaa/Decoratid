using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Store;
using Sandbox.Store.Broker;
using Sandbox.Thingness;

namespace Sandbox.Conditions
{
    ///// <summary>
    ///// Takes an IConditionOf and converts it to an ICondition by deferring resolution of the context argument until
    ///// condition evaluation.
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public class DeferredCondition<T> : ICondition
    //{
    //    #region Ctor
    //    protected DeferredCondition() { }
    //    public DeferredCondition(Func<T> contextFactory, IConditionOf<T> conditionToDefer)
    //    {
    //        Condition.Requires(contextFactory).IsNotNull();
    //        Condition.Requires(conditionToDefer).IsNotNull();
    //        this.ContextFactory = contextFactory;
    //        this.ConditionToDefer = conditionToDefer;
    //    }
    //    #endregion

    //    #region Properties
    //    public Func<T> ContextFactory { get; protected set; }
    //    public IConditionOf<T> ConditionToDefer { get; protected set; }
    //    #endregion

    //    #region ICondition
    //    public bool? Evaluate()
    //    {
    //        return this.ConditionToDefer.Evaluate(this.ContextFactory());
    //    }
    //    #endregion
    //}

    ///// <summary>
    ///// like a deferred condition, only using a storedobject and not a function
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public class StoredCondition<T> : ICondition
    //    where T : IHasId
    //{
    //    #region Ctor
    //    public StoredCondition(IStoredObjectPointer storedObject, IConditionOf<T> conditionToDefer)
    //        :base()
    //    {
    //        Condition.Requires(storedObject).IsNotNull();
    //        Condition.Requires(conditionToDefer).IsNotNull();
    //        this.StoredObject = storedObject;
    //        this.ConditionToDefer = conditionToDefer;
    //    }
    //    #endregion

    //    #region Properties
    //    public IStoredObjectPointer StoredObject { get; protected set; }
    //    public IConditionOf<T> ConditionToDefer { get; protected set; }
    //    #endregion

    //    #region ICondition
    //    public bool? Evaluate()
    //    {
    //        T obj = (T)this.StoredObject.GetStoredObject();
    //        return this.ConditionToDefer.Evaluate(obj);
    //    }
    //    #endregion
    //}

    /// <summary>
    /// Takes an IConditionOf and converts it to an ICondition by keeping context T as state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ContextualCondition<T> : ICondition, IHasContext<T>, ICloneableCondition
    {
        #region Ctor
        protected ContextualCondition() { }
        public ContextualCondition(T context, IConditionOf<T> conditionToDefer)
        {
            Condition.Requires(conditionToDefer).IsNotNull();
            this.Context = context;
            this.ConditionToDefer = conditionToDefer;
        }
        #endregion

        #region Properties
        public T Context{ get; set; }
        object IHasContext.Context { get { return this.Context; } set { if (value != null && typeof(T).IsAssignableFrom(value.GetType())) { this.Context = (T)value; } } }
        public IConditionOf<T> ConditionToDefer { get; protected set; }
        #endregion

        #region ICondition
        public bool? Evaluate()
        {
            return this.ConditionToDefer.Evaluate(this.Context);
        }
        #endregion

        public virtual ICondition Clone()
        {
            return new ContextualCondition<T>(this.Context, this.ConditionToDefer);
        }

        #region Static Fluent Methods
        public static ICondition New<TArg>(TArg context, IConditionOf<TArg> conditionToDefer)
        {
            return new ContextualCondition<TArg>(context, conditionToDefer);
        }
        #endregion
    }

    /// <summary>
    /// Takes an IConditionOf and converts it to an ICondition by keeping context T as state, and provides mutability
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MutableContextualCondition<T> : ContextualCondition<T>, IMutableCondition, ICloneableCondition
    {
        #region Ctor
        protected MutableContextualCondition() { }
        public MutableContextualCondition(T context, IConditionOf<T> conditionToDefer, Func<T,T> mutateStrategy)
            :base(context, conditionToDefer)
        {
            Condition.Requires(mutateStrategy).IsNotNull();
            this.MutateStrategy = mutateStrategy;
        }
        #endregion

        #region Properties
        protected Func<T,T> MutateStrategy { get; set; }
        #endregion

        #region Methods
        public void Mutate()
        {
            if (MutateStrategy == null) { return; }

            this.Context = this.MutateStrategy(this.Context);
        }
        public override ICondition Clone()
        {
            return new MutableContextualCondition<T>(this.Context, this.ConditionToDefer, this.MutateStrategy);
        }
        #endregion

        #region Static Fluent Methods
        public static ICondition New<TArg>(TArg context, IConditionOf<TArg> conditionToDefer, Func<TArg, TArg> mutateStrategy)
        {
            return new MutableContextualCondition<TArg>(context, conditionToDefer, mutateStrategy);
        }
        #endregion
    }
}
