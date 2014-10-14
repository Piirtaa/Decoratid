using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Throttling
{
    [Serializable]
    public class ThrottlingLogicDecoration : DecoratedLogicBase, IHasThrottle
    {
        #region Ctor
        public ThrottlingLogicDecoration(ILogic decorated, int maxConcurrency)
            : base(decorated)
        {
            this.Throttle = new NaturalThrottle(maxConcurrency);
        }
        #endregion

        #region ISerializable
        protected ThrottlingLogicDecoration(SerializationInfo info, StreamingContext context)
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
        public override void Perform()
        {
            this.Throttle.Perform(() =>
            {
                Decorated.Perform();
            });
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new ThrottlingLogicDecoration(thing, this.Throttle.ConcurrencyLimit);
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

    public static class ThrottlingLogicDecorationExtensions
    {
        public static ThrottlingLogicDecoration Throttle(this ILogic decorated, int maxConcurrency)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ThrottlingLogicDecoration(decorated, maxConcurrency);
        }
    }
}
