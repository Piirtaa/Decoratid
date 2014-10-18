using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Adjusting
{
    public interface IAdjustingLogic : IDecoratedLogic, IAdjustment<ILogic>
    {
    }

    [Serializable]
    public class AdjustingLogicDecoration : DecoratedLogicBase, IAdjustingLogic
    {
        #region Ctor
        public AdjustingLogicDecoration(ILogic decorated, LogicOfTo<ILogic, ILogic> adjustment)
            : base(decorated)
        {
            Condition.Requires(adjustment).IsNotNull();
            this.AdjustmentLogic = adjustment;
        }
        #endregion

        #region ISerializable
        protected AdjustingLogicDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.AdjustmentLogic = (LogicOfTo<ILogic, ILogic>)info.GetValue("Adjustment", typeof(LogicOfTo<ILogic, ILogic>));
            this.AdjustedValue = (ILogic)info.GetValue("AdjustedValue", typeof(ILogic));
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
            info.AddValue("Adjustment", this.AdjustmentLogic);
            info.AddValue("AdjustedValue", this.AdjustedValue);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion


        #region IAdjustingLogic
        public LogicOfTo<ILogic, ILogic> AdjustmentLogic { get; private set; }
        public ILogic AdjustedValue { get; private set; }
        #endregion

        #region Methods
        public override void Perform()
        {
            var adjustment = this.AdjustmentLogic.CloneAndPerform(this.Decorated.AsNaturalValue());
            this.AdjustedValue = adjustment;
            this.AdjustedValue.Perform(); 
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new AdjustingLogicDecoration(thing, this.AdjustmentLogic);
        }
        #endregion
    }

    public static class AdjustingLogicDecorationExtensions
    {
        public static AdjustingLogicDecoration Adjust(this ILogic decorated, LogicOfTo<ILogic, ILogic> adjustment)
        {
            return new AdjustingLogicDecoration(decorated, adjustment);
        }
    }
}
