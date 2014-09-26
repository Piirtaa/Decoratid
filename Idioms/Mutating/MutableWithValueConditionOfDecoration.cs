using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.ValueOfing;
using Decoratid.Core.Conditional;
using Decoratid.Core.Conditional.Of;
using Decoratid.Idioms.WithValuing;

namespace Decoratid.Idioms.Mutating
{
    /// <summary>
    /// marks a condition as being Mutable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMutableCondition : ICondition, IMutable
    {
    }

    /// <summary>
    /// decorates by adding a mutation strategy to a ConditionOf WithValue instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class MutableWithValueConditionOfDecoration<T> : DecoratedConditionOfBase<T>, IMutableCondition
    {
        #region Ctor
        public MutableWithValueConditionOfDecoration(WithValueConditionOfDecoration<T> decorated, LogicOfTo<T, T> mutateStrategy)
            : base(decorated)
        {
            Condition.Requires(mutateStrategy).IsNotNull();
            this.MutationStrategy = mutateStrategy;
        }
        #endregion

        #region Fluent Static
        public static MutableWithValueConditionOfDecoration<T> New(WithValueConditionOfDecoration<T> decorated, LogicOfTo<T, T> mutateStrategy)
        {
            return new MutableWithValueConditionOfDecoration<T>(decorated, mutateStrategy);
        }
        #endregion

        #region ISerializable
        protected MutableWithValueConditionOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
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

        #region ICondition
        /// <summary>
        /// mutates prior to evaluation
        /// </summary>
        /// <returns></returns>
        public override bool? Evaluate()
        {
            this.Mutate();
            WithValueConditionOfDecoration<T> dec = (WithValueConditionOfDecoration<T>)this.Decorated;
            return dec.Evaluate();
        }
        #endregion

        #region IMutableCondition
        public virtual void Mutate()
        {
            //grab the context
            WithValueConditionOfDecoration<T> dec = (WithValueConditionOfDecoration<T>)this.Decorated;
            var val = dec.Context;
          
            //mutate it 
            var newVal = this.MutationStrategy.CloneAndPerform(val);
            
            //replace
            dec.Context = newVal.AsNaturalValue();
            
        }
        #endregion
    }

    public static partial class Extensions
    {
        /// <summary>
        /// decorates with polyfacingness if it's not already there
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueOf"></param>
        /// <param name="rootFace"></param>
        /// <returns></returns>
        public static MutableWithValueConditionOfDecoration<T> Mutating<T>(this WithValueConditionOfDecoration<T> decorated, LogicOfTo<T, T> mutateStrategy)
        {
            Condition.Requires(decorated).IsNotNull();

            if (decorated is WithValueConditionOfDecoration<T>)
            {
                var pf = decorated as WithValueConditionOfDecoration<T>;
                return pf;
            }

            return new WithValueConditionOfDecoration<T>(decorated, rootFace);
        }
    }
}
