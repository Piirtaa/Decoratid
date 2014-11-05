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
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion


        #region IAdjustingLogic
        public LogicOfTo<ILogic, ILogic> AdjustmentLogic { get; private set; }
        #endregion

        #region Methods
        public override void Perform()
        {
            this.AdjustmentLogic.Context = this.Decorated.AsNaturalValue();
            this.AdjustmentLogic.Perform();
            var adjusted = this.AdjustmentLogic.Result;
            adjusted.Perform();
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
        public static AdjustingLogicDecoration Adjust(this ILogic decorated, Func<ILogic, ILogic> adjustment)
        {
            return new AdjustingLogicDecoration(decorated, adjustment.MakeLogicOfTo());
        }
    }
}
