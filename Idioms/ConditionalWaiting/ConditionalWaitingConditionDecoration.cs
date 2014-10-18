using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.ConditionalWaiting
{

    /// <summary>
    /// kacks on evaluation if the validating condition isn't true
    /// </summary>
    /// 
    [Serializable]
    public class ConditionalWaitingConditionDecoration : DecoratedConditionBase, IHasConditionalWaiter
    {
        #region Ctor
        public ConditionalWaitingConditionDecoration(ICondition decorated, ICondition waitCondition, ICondition stopWaitingCondition)
            : base(decorated)
        {
            this.Waiter = new ConditionalWaiter(waitCondition, stopWaitingCondition);
        }
        #endregion

        #region ISerializable
        protected ConditionalWaitingConditionDecoration(SerializationInfo info, StreamingContext context)
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
        public ICondition WaitCondition { get { return this.Waiter.WaitCondition; } }
        public ICondition StopWaitingCondition { get { return this.Waiter.StopWaitingCondition; } }
        public bool WaitAround() { return this.Waiter.WaitAround(); }
        #endregion


        #region Methods
        public override bool? Evaluate()
        {
            var waitRV = this.Waiter.WaitAround();
            if (!waitRV)
                throw new InvalidOperationException("wait stopped");

            return base.Evaluate();
        }
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            return new ConditionalWaitingConditionDecoration(thing, this.WaitCondition, this.StopWaitingCondition);
        }
        #endregion
    }

    public static class ConditionalWaitingConditionDecorationExtensions
    {
        public static ConditionalWaitingConditionDecoration WaitUntil(ICondition decorated, ICondition waitCondition, ICondition stopWaitingCondition)
        {
            return new ConditionalWaitingConditionDecoration(decorated, waitCondition, stopWaitingCondition);
        }
    }
}
