using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Expiring
{
    /// <summary>
    /// kacks on evaluation if the validating condition isn't true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    [Serializable]
    public class ExpiringValueOfDecoration<T> : DecoratedValueOfBase<T>, IHasExpirable
    {
        #region Ctor
        public ExpiringValueOfDecoration(IValueOf<T> decorated, IExpirable expirable)
            : base(decorated)
        {
            Condition.Requires(expirable).IsNotNull();
            this.Expirable = expirable;
        }
        #endregion

        #region ISerializable
        protected ExpiringValueOfDecoration(SerializationInfo info, StreamingContext context)
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
        public override T GetValue()
        {
            if (this.IsExpired())
                throw new InvalidOperationException("expired");

            return Decorated.GetValue();
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new ExpiringValueOfDecoration<T>(thing, this.Expirable);
        }
        #endregion
    }

    public static class ExpiringValueOfDecorationExtensions
    {
        public static ExpiringValueOfDecoration<T> HasExpirable<T>(IValueOf<T> decorated, IExpirable expirable)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ExpiringValueOfDecoration<T>(decorated, expirable);
        }
    }
}
