using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Expiring
{
    /// <summary>
    /// kacks on evaluation if the validating condition isn't true
    /// </summary>
    /// 
    [Serializable]
    public class ExpiringLogicDecoration : DecoratedLogicBase, IExpirable
    {
        #region Ctor
        public ExpiringLogicDecoration(ILogic decorated, IExpirable expirable)
            : base(decorated)
        {
            Condition.Requires(expirable).IsNotNull();
            this.Expirable = expirable;
        }
        #endregion

        #region ISerializable
        protected ExpiringLogicDecoration(SerializationInfo info, StreamingContext context)
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
        public IExpirable Expirable { get; private set; }
        #endregion

        #region IExpirable
        public bool IsExpired()
        {
            return this.Expirable.IsExpired();
        }
        #endregion

        #region Methods
        public override void Perform()
        {
            if (this.IsExpired())
                throw new InvalidOperationException("expired");

            Decorated.Perform();
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new ExpiringLogicDecoration(thing, this.Expirable);
        }
        #endregion
    }

    public static class ExpiringLogicDecorationExtensions
    {
        public static ExpiringLogicDecoration HasExpirable(ILogic decorated, IExpirable expirable)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ExpiringLogicDecoration(decorated, expirable);
        }
    }
}
