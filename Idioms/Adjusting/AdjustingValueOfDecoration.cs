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
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public interface IAdjustingValueOf<T> : IValueOf<T>
    {
        LogicOfTo<T, T> Adjustment { get; }
        T AdjustedValue { get; }
    }

    /// <summary>
    /// is a ValueOf decoration that evaluates the decorated ValueOf and then scrubs it according to some strategy
    /// </summary>
    [Serializable]
    public class AdjustedValueOfDecoration<T> : DecoratedValueOfBase<T>, IAdjustingValueOf<T>
    {
        #region Ctor
        public AdjustedValueOfDecoration(IValueOf<T> decorated, LogicOfTo<T, T> ScrubbingStrategy)
            : base(decorated)
        {
            Condition.Requires(ScrubbingStrategy).IsNotNull();
            this.Adjustment = ScrubbingStrategy;
        }
        #endregion

        #region ISerializable
        protected AdjustedValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Adjustment = (LogicOfTo<T, T>)info.GetValue("Adjustment", typeof(LogicOfTo<T, T>));
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
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public LogicOfTo<T, T> Adjustment { get; private set; }
        public T AdjustedValue { get; private set; } 
        #endregion

        #region Methods
        public override T GetValue()
        {
            var oldValue = Decorated.GetValue();
            var rv = this.Adjustment.CloneAndPerform(oldValue.AsNaturalValue());
            this.ScrubbedValue = rv;
            return rv;
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new AdjustedValueOfDecoration<T>(thing, this.Adjustment);
        }
        #endregion
    }

    public static partial class AdjustedValueOfExtensions
    {
        /// <summary>
        /// applies an adjustment/mutation to a ValueOf but doesn't change the underlying value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueOf"></param>
        /// <param name="mutateStrategy"></param>
        /// <returns></returns>
        public static AdjustedValueOfDecoration<T> Adust<T>(this IValueOf<T> valueOf, LogicOfTo<T, T> mutateStrategy)
        {
            Condition.Requires(valueOf).IsNotNull();
            Condition.Requires(mutateStrategy).IsNotNull();
            return new AdjustedValueOfDecoration<T>(valueOf, mutateStrategy);
        }
    }

}
