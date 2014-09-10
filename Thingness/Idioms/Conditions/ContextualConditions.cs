using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Store;
using Decoratid.Thingness.Idioms.Store.Broker;
using Decoratid.Thingness;
using Decoratid.Thingness.Idioms.Logics;
using Decoratid.Thingness.Idioms.ValuesOf;
using Decoratid.Extensions;

namespace Decoratid.Thingness.Idioms.Conditions
{
    /// <summary>
    /// Takes an IConditionOf and converts it to an ICondition by keeping context IValueOf as state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ContextualCondition<T> : ICondition, IHasContext<IValueOf<T>>, ICloneableCondition
    {
        #region Ctor
        protected ContextualCondition() { }
        public ContextualCondition(IValueOf<T> context, IConditionOf<T> conditionOf)
        {
            Condition.Requires(context).IsNotNull();
            Condition.Requires(conditionOf).IsNotNull();
            this.Context = context;
            this.ConditionOf = conditionOf;
        }
        #endregion

        #region Properties
        public IValueOf<T> Context { get; set; }
        object IHasContext.Context { get { return this.Context; } set { this.Context = (IValueOf<T>)value; } }
        public IConditionOf<T> ConditionOf { get; protected set; }
        #endregion

        #region ICondition
        public bool? Evaluate()
        {
            var val = this.Context.GetValue();

            //clone the condition if we can
            if (this.ConditionOf is ICloneableCondition)
            {
                ICloneableCondition cloneCond = (ICloneableCondition)this.ConditionOf;
                IConditionOf<T> clone = (IConditionOf<T>)cloneCond.Clone();
                return clone.Evaluate(val);
            }
            return this.ConditionOf.Evaluate(val);
        }
        #endregion

        public virtual ICondition Clone()
        {
            return new ContextualCondition<T>(this.Context, this.ConditionOf);
        }

        #region Static Fluent Methods
        public static ICondition New(IValueOf<T> context, IConditionOf<T> conditionOf)
        {
            return new ContextualCondition<T>(context, conditionOf);
        }
        #endregion

        //#region ISerializable
        //protected ContextualCondition(SerializationInfo info, StreamingContext context)
        //{
        //    this.ConditionOf = (IConditionOf<T>)info.GetValue("_ConditionOf", typeof(IConditionOf<T>));
        //    this.Context = (IValueOf<T>)info.GetValue("_Context", typeof(IValueOf<T>));
        //}
        //public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("_ConditionOf", this.ConditionOf);
        //    info.AddValue("_Context", this.Context);
        //}
        //#endregion
    }


    /// <summary>
    /// Takes an IConditionOf and converts it to an ICondition by keeping context IValueOf as state, and provides mutability
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MutableContextualCondition<T> : ContextualCondition<T>, IMutableCondition, ICloneableCondition
    {
        #region Ctor
        protected MutableContextualCondition() { }
        public MutableContextualCondition(IValueOf<T> context, IConditionOf<T> conditionOf, LogicOfTo<T, T> mutateStrategy)
            : base(context, conditionOf)
        {
            Condition.Requires(mutateStrategy).IsNotNull();
            this.MutateStrategy = mutateStrategy;
        }
        /// <summary>
        /// uses delegates and not LogicWrappers
        /// </summary>
        /// <param name="context"></param>
        /// <param name="conditionOf"></param>
        /// <param name="mutateStrategy"></param>
        public MutableContextualCondition(IValueOf<T> context, IConditionOf<T> conditionOf, Func<T, T> mutateStrategy)
            : this(context, conditionOf, mutateStrategy.MakeLogicOfTo())
        {
        }
        #endregion

        #region Properties
        protected LogicOfTo<T, T> MutateStrategy { get; set; }
        #endregion

        #region Methods
        public void Mutate()
        {
            if (MutateStrategy == null) { return; }
            var res = this.MutateStrategy.CloneAndPerform(this.Context);
            this.Context = res.ValueOf();
        }
        public override ICondition Clone()
        {
            return new MutableContextualCondition<T>(this.Context, this.ConditionOf, this.MutateStrategy);
        }
        #endregion

        #region Static Fluent Methods
        public static ICondition New(IValueOf<T> context, IConditionOf<T> conditionOf, LogicOfTo<T, T> mutateStrategy)
        {
            return new MutableContextualCondition<T>(context, conditionOf, mutateStrategy);
        }
        public static ICondition New(IValueOf<T> context, IConditionOf<T> conditionOf, Func<T, T> mutateStrategy)
        {
            return new MutableContextualCondition<T>(context, conditionOf, mutateStrategy);
        }
        #endregion

        //#region ISerializable
        //protected MutableContextualCondition(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //    info.AddValue("_MutateStrategy", this.MutateStrategy);
        //}
        //public override void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    base.GetObjectData(info, context);
        //    this.MutateStrategy = (LogicOfTo<T, T>)info.GetValue("_MutateStrategy", typeof(LogicOfTo<T, T>));
        //}
        //#endregion
    }
}
