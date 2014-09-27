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
    public class ConditionalWaitingValueOfDecoration<T> : DecoratedValueOfBase<T>, IHasWaitCondition
    {
        #region Ctor
        public ConditionalWaitingValueOfDecoration(IValueOf<T> decorated, ICondition waitCondition, ICondition stopWaitingCondition)
            : base(decorated)
        {
            this.Waiter = new ConditionalWaiter(waitCondition, stopWaitingCondition);
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

        #region IHasWaitCondition
        private ConditionalWaiter Waiter { get; set; }
        public ICondition WaitCondition { get { return this.Waiter.WaitCondition; } }
        public ICondition StopWaitingCondition { get { return this.Waiter.StopWaitingCondition; } }
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
            return new ConditionalWaitingValueOfDecoration<T>(thing, this.WaitCondition, this.StopWaitingCondition);
        }
        #endregion
    }

    public static class ConditionalWaitingValueOfDecorationExtensions
    {
        public static ConditionalWaitingValueOfDecoration<T> KackUntil<T>(IValueOf<T> decorated, ICondition waitCondition, ICondition stopWaitingCondition)
        {
            return new ConditionalWaitingValueOfDecoration<T>(decorated, waitCondition, stopWaitingCondition);
        }
    }
}
