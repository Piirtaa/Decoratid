using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Expiring
{

    /// <summary>
    /// kacks on evaluation if the validating condition isn't true
    /// </summary>
    [Serializable]
    public class ExpiringConditionDecoration : DecoratedConditionBase, IHasExpirable
    {
        #region Ctor
        public ExpiringConditionDecoration(ICondition decorated, IExpirable expirable)
            : base(decorated)
        {
            Condition.Requires(expirable).IsNotNull();
            this.Expirable = expirable;
        }
        #endregion

        #region ISerializable
        protected ExpiringConditionDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Expirable = (IExpirable)info.GetValue("Expirable", typeof(IExpirable));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Expirable", this.Expirable); ;
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasExpirable
        public IExpirable Expirable { get; set; }
        #endregion

        #region IExpirable
        public virtual bool IsExpired()
        {
            return this.Expirable.IsExpired();
        }
        #endregion

        #region Overrides
        public override bool? Evaluate()
        {
            if(this.IsExpired())
                throw new InvalidOperationException("expired");

            return base.Evaluate();
        }
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            return new ExpiringConditionDecoration(thing, this.Expirable);
        }
        #endregion
    }

    public static class ExpiringConditionDecorationExtensions
    {
        public static ExpiringConditionDecoration HasExpirable(this ICondition decorated, IExpirable expirable)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ExpiringConditionDecoration(decorated, expirable);
        }
        public static ExpiringConditionDecoration HasExpirable(this ICondition decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ExpiringConditionDecoration(decorated, NaturalFalseExpirable.New());
        }
    }
}
