using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.ConditionalWaiting
{
    /// <summary>
    /// kacks on evaluation if the validating condition isn't true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    [Serializable]
    public class ConditionalWaitingValueOfDecoration<T> : DecoratedValueOfBase<T>, IHasConditionalWaiter
    {
        #region Ctor
        public ConditionalWaitingValueOfDecoration(IValueOf<T> decorated, ICondition Condition, ICondition stopWaitingCondition = null)
            : base(decorated)
        {
            this.Waiter = new ConditionalWaiter(Condition, stopWaitingCondition);
        }
        #endregion

        #region ISerializable
        protected ConditionalWaitingValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Waiter = (ConditionalWaiter)info.GetValue("Waiter", typeof(ConditionalWaiter));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Waiter", this.Waiter); ;
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasConditionalWaiter
        public IConditionalWaiter Waiter { get; set; }
        #endregion

        #region Methods
        public override T GetValue()
        {
            var waitRV = this.Waiter.WaitAround();
            if (!waitRV)
                throw new InvalidOperationException("wait stopped");

            return Decorated.GetValue();
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new ConditionalWaitingValueOfDecoration<T>(thing, this.Waiter.Condition, this.Waiter.StopWaitingCondition);
        }
        #endregion
    }

    public static class ConditionalWaitingValueOfDecorationExtensions
    {
        public static ConditionalWaitingValueOfDecoration<T> WaitUntil<T>(this IValueOf<T> decorated, ICondition Condition, ICondition stopWaitingCondition)
        {
            return new ConditionalWaitingValueOfDecoration<T>(decorated, Condition, stopWaitingCondition);
        }
    }
}
