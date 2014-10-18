using CuttingEdge.Conditions;
using Decoratid.Core;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Adjusting
{
    /// <summary>
    /// is a ValueOf decoration that evaluates the decorated ValueOf and then scrubs it according to some strategy
    /// </summary>
    /// <remarks>
    /// Hold on!  Why would we do this? So we can chain adjustments on a value.
    /// Also can be repurposed for audit and any general decoration.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public interface IAdjustingValueOf<T> : IValueOf<T>, IAdjustment<T>
    {
    }

    /// <summary>
    /// is a ValueOf decoration that evaluates the decorated ValueOf and then scrubs it according to some strategy
    /// </summary>
    [Serializable]
    public class AdjustingValueOfDecoration<T> : DecoratedValueOfBase<T>, IAdjustingValueOf<T>
    {
        #region Ctor
        public AdjustingValueOfDecoration(IValueOf<T> decorated, LogicOfTo<T, T> adjustment)
            : base(decorated)
        {
            Condition.Requires(adjustment).IsNotNull();
            this.AdjustmentLogic = adjustment;
        }
        #endregion

        #region ISerializable
        protected AdjustingValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.AdjustmentLogic = (LogicOfTo<T, T>)info.GetValue("Adjustment", typeof(LogicOfTo<T, T>));
            this.AdjustedValue = (T)info.GetValue("AdjustedValue", typeof(T));
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

        #region Properties
        public LogicOfTo<T, T> AdjustmentLogic { get; private set; }
        public T AdjustedValue { get; private set; }
        #endregion

        #region Methods
        public override T GetValue()
        {
            var oldValue = Decorated.GetValue();
            var rv = this.AdjustmentLogic.CloneAndPerform(oldValue.AsNaturalValue());
            this.AdjustedValue = rv;
            return rv;
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new AdjustingValueOfDecoration<T>(thing, this.AdjustmentLogic);
        }
        #endregion
    }

    public static partial class AdjustingValueOfDecorationExtensions
    {
        public static AdjustingValueOfDecoration<T> Adjust<T>(this IValueOf<T> valueOf, LogicOfTo<T, T> adjustment)
        {
            Condition.Requires(valueOf).IsNotNull();
            Condition.Requires(adjustment).IsNotNull();
            return new AdjustingValueOfDecoration<T>(valueOf, adjustment);
        }
    }

}
