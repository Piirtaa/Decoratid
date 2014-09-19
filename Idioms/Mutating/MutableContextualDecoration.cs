using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.Core.ValueOfing;
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Idioms.Core.Conditional.Of;

namespace Decoratid.Idioms.Mutating
{
    /// <summary>
    /// marks a condition as being Mutable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMutableCondition : ICondition
    {
    }

    /// <summary>
    /// decorates by adding a mutation strategy to a Contextual decoration instance.
    /// Typically used when we have an IConditionOf we've made contextual by supplying an argument, and then
    /// we supply some logic by which the argument might mutate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class MutableConditionDecoration<T> : DecoratedConditionOfBase<T>, IMutableCondition
    {
        #region Ctor
        public MutableConditionDecoration(WithValueDecoration<T> decorated, LogicOfTo<T, T> mutateStrategy)
            : base(decorated)
        {
            Condition.Requires(mutateStrategy).IsNotNull();
            this.MutationStrategy = mutateStrategy;
        }
        #endregion

        #region Fluent Static
        public static MutableConditionDecoration<T> New(WithValueDecoration<T> decorated, LogicOfTo<T, T> mutateStrategy)
        {
            return new MutableConditionDecoration<T>(decorated, mutateStrategy);
        }
        #endregion

        #region ISerializable
        protected MutableConditionDecoration(SerializationInfo info, StreamingContext context)
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
        public override bool? Evaluate()
        {
            this.Mutate();
            WithValueDecoration<T> dec = (WithValueDecoration<T>)this.Decorated;
            return dec.Evaluate();
        }
        #endregion

        #region IMutableCondition
        public virtual void Mutate()
        {
            //grab the context
            WithValueDecoration<T> dec = (WithValueDecoration<T>)this.Decorated;
            var val = dec.Context;
          
            //mutate it 
            var newVal = this.MutationStrategy.CloneAndPerform(val);
            
            //replace
            dec.Context = newVal.AsNaturalValue();
            
        }
        #endregion
    }
}
