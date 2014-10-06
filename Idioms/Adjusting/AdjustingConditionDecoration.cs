using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Adjusting
{
    public interface IAdjustingCondition : IDecoratedCondition
    {
        LogicOfTo<ICondition, ICondition> Adjustment { get; }
        ICondition AdjustedValue { get; }
    }

    [Serializable]
    public class AdjustingConditionDecoration : DecoratedConditionBase, IAdjustingCondition
    {
        #region Ctor
        public AdjustingConditionDecoration(ICondition decorated, LogicOfTo<ICondition, ICondition> adjustment)
            : base(decorated)
        {
            Condition.Requires(adjustment).IsNotNull();
            this.Adjustment = adjustment;
        }
        #endregion

        #region ISerializable
        protected AdjustingConditionDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Adjustment = (LogicOfTo<ICondition, ICondition>)info.GetValue("Adjustment", typeof(LogicOfTo<ICondition, ICondition>));
            this.AdjustedValue = (ICondition)info.GetValue("AdjustedValue", typeof(ICondition));
        }
        /// <summary>
        /// since we don't want to expose ISerializable concerns publicly, we use a virtual protected
        /// helper function that does the actual implementation of ISerializable, and is called by the
        /// explicit interface implementation of GetObjectData.  This is the method to be overridden in 
        /// derived classes.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Adjustment", this.Adjustment);
            info.AddValue("AdjustedValue", this.AdjustedValue);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IAdjustingCondition
        public LogicOfTo<ICondition, ICondition> Adjustment { get; private set; }
        public ICondition AdjustedValue { get; private set; }
        #endregion

        #region Methods
        public override bool? Evaluate()
        {
            var adjustment = this.Adjustment.CloneAndPerform(this.Decorated.AsNaturalValue());
            this.AdjustedValue = adjustment;
            var rv = this.AdjustedValue.Evaluate();
            return rv;
        }
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            return new AdjustingConditionDecoration(thing, this.Adjustment);
        }
        #endregion
    }

    public static class AdjustingConditionDecorationExtensions
    {
        public static AdjustingConditionDecoration Adjust(this ICondition decorated, LogicOfTo<ICondition, ICondition> adjustment)
        {
            return new AdjustingConditionDecoration(decorated, adjustment);
        }
    }
}
