using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional.Core;
using Decoratid.Idioms.Core.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Core.Conditional.Decorations
{
    /// <summary>
    /// marks a condition as being Mutable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMutableCondition : ICondition
    {
        void Mutate();
    }

    /// <summary>
    /// decorates by adding a mutation strategy to a ConditionOfCondition instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class MutableConditionOfConditionDecoration<T> : DecoratedConditionBase, IMutableCondition
    {
        #region Ctor
        public MutableConditionOfConditionDecoration(ConditionOfCondition<T> decorated, LogicOfTo<T, T> mutateStrategy)
            : base(decorated)
        {
            Condition.Requires(mutateStrategy).IsNotNull();
            this.MutationStrategy = mutateStrategy;
        }
        #endregion

        #region Fluent Static
        public static MutableConditionOfConditionDecoration<T> New(ConditionOfCondition<T> decorated, LogicOfTo<T, T> mutateStrategy)
        {
            return new MutableConditionOfConditionDecoration<T>(decorated, mutateStrategy);
        }
        #endregion

        #region ISerializable
        protected MutableConditionOfConditionDecoration(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.MutationStrategy = (LogicOfTo<T, T>)info.GetValue("_MutationStrategy", typeof(LogicOfTo<T, T>));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_MutationStrategy", this.MutationStrategy);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        protected LogicOfTo<T, T> MutationStrategy { get; set; }
        #endregion

        #region IMutableConditionOf
        public override bool? Evaluate()
        {

            return base.Decorated.Evalu
            ate(context);
        }
        #endregion

        #region IMutableConditionOf
        public virtual void Mutate(T obj)
        {

        }
        #endregion
    }
}
