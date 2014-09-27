using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.ConditionalWaiting
{
    /// <summary>
    /// kacks on evaluation if the validating condition isn't true
    /// </summary>
    /// 
    [Serializable]
    public class ConditionalWaitingLogicDecoration : DecoratedLogicBase, IHasWaitCondition
    {
        #region Ctor
        public ConditionalWaitingLogicDecoration(ILogic decorated, ICondition waitCondition, ICondition stopWaitingCondition)
            : base(decorated)
        {
            this.Waiter = new ConditionalWaiter(waitCondition, stopWaitingCondition);
        }
        #endregion

        #region ISerializable
        protected ConditionalWaitingLogicDecoration(SerializationInfo info, StreamingContext context)
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
        public override void Perform()
        {
            var waitRV = this.Waiter.WaitAround();
            if (!waitRV)
                throw new InvalidOperationException("wait stopped");

            Decorated.Perform();
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new ConditionalWaitingLogicDecoration(thing, this.WaitCondition, this.StopWaitingCondition);
        }
        #endregion
    }

    public static class ConditionalWaitingLogicDecorationExtensions
    {
        public static ConditionalWaitingLogicDecoration WaitUntil(ILogic decorated, ICondition waitCondition, ICondition stopWaitingCondition)
        {
            return new ConditionalWaitingLogicDecoration(decorated, waitCondition, stopWaitingCondition);
        }
    }
}
