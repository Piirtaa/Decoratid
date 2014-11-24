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
    public class ConditionalWaitingLogicDecoration : DecoratedLogicBase, IHasConditionalWaiter
    {
        #region Ctor
        public ConditionalWaitingLogicDecoration(ILogic decorated, ICondition Condition, ICondition stopWaitingCondition =null)
            : base(decorated)
        {
            this.Waiter = new ConditionalWaiter(Condition, stopWaitingCondition);
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

        #region IHasConditionalWaiter
        public IConditionalWaiter Waiter { get; set; }
        #endregion

        #region Methods
        public override ILogic Perform(object context = null)
        {
            var waitRV = this.Waiter.WaitAround();
            if (!waitRV)
                throw new InvalidOperationException("wait stopped");

            return Decorated.Perform(context);
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new ConditionalWaitingLogicDecoration(thing,  this.Waiter.Condition, this.Waiter.StopWaitingCondition);
        }
        #endregion
    }

    public static class ConditionalWaitingLogicDecorationExtensions
    {
        public static ConditionalWaitingLogicDecoration WaitUntil(this ILogic decorated, ICondition Condition, ICondition stopWaitingCondition)
        {
            return new ConditionalWaitingLogicDecoration(decorated, Condition, stopWaitingCondition);
        }
    }
}
