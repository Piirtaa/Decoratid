using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Throttling
{

    [Serializable]
    public class ThrottlingValueOfDecoration<T> : DecoratedValueOfBase<T>, IHasThrottle
    {
        #region Ctor
        public ThrottlingValueOfDecoration(IValueOf<T> decorated, int maxConcurrency)
            : base(decorated)
        {
            this.Throttle = new NaturalThrottle(maxConcurrency);
        }
        #endregion

        #region ISerializable
        protected ThrottlingValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var limit = info.GetInt32("Throttle");
            this.Throttle = new NaturalThrottle(limit);
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Throttle", this.Throttle.ConcurrencyLimit); ;
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasThrottle
        public IThrottle Throttle { get; private set; }

        #endregion

        #region Methods
        public override T GetValue()
        {
            T rv = default(T);

            this.Throttle.Perform(() =>
            {
                rv = Decorated.GetValue();
            });

            return rv;
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new ThrottlingValueOfDecoration<T>(thing, this.Throttle.ConcurrencyLimit);
        }
        #endregion

    }

    public static class ThrottlingValueOfDecorationExtensions
    {
        public static ThrottlingValueOfDecoration<T> Throttle<T>(this IValueOf<T> decorated, int maxConcurrency)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ThrottlingValueOfDecoration<T>(decorated, maxConcurrency);
        }
    }
}
