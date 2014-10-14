using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Throttling
{

    [Serializable]
    public class ThrottlingConditionDecoration : DecoratedConditionBase, IHasThrottle
    {
        #region Ctor
        public ThrottlingConditionDecoration(ICondition decorated, int maxConcurrency)
            : base(decorated)
        {
            this.Throttle = new NaturalThrottle(maxConcurrency);
        }
        #endregion

        #region ISerializable
        protected ThrottlingConditionDecoration(SerializationInfo info, StreamingContext context)
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
        public override bool? Evaluate()
        {
            bool? rv = null;

            this.Throttle.Perform(() =>
            {
                rv = base.Evaluate();
            });
            return rv;
        }
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            return new ThrottlingConditionDecoration(thing, this.Throttle.ConcurrencyLimit);
        }
        #endregion

        #region IThrottle
        public int ConcurrencyLimit
        {
            get { return this.Throttle.ConcurrencyLimit; }
        }

        public void Reset()
        {
            this.Throttle.Reset();
        }

        public void Perform(Action action)
        {
            this.Throttle.Perform(action);
        }
        #endregion
    }

    public static class ThrottlingConditionDecorationExtensions
    {
        public static ThrottlingConditionDecoration Throttle(this ICondition decorated, int maxConcurrency)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ThrottlingConditionDecoration(decorated, maxConcurrency);
        }
    }
}
