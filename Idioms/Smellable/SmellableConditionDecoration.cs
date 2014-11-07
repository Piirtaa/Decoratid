using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using System;
using System.Runtime.Serialization;
using Decoratid.Idioms.ObjectGraphing;

namespace Decoratid.Idioms.Smellable
{
    [Serializable]
    public class SmellableConditionDecoration : DecoratedConditionBase, ISmellable
    {
        #region Ctor
        public SmellableConditionDecoration(ICondition decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected SmellableConditionDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region ISmellable
        public bool? SmellsGood()
        {
            //if it can't be serialized back to itself it's a problem
            try
            {
                string dat = this.GraphSerializeWithDefaults();
                var obj = dat.GraphDeserializeWithDefaults();
                string dat2 = obj.GraphSerializeWithDefaults();
                return dat.Equals(dat2);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Overrides
        public override bool? Evaluate()
        {
            this.SmellCheck();

            return base.Evaluate();
        }
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            return new SmellableConditionDecoration(thing);
        }
        #endregion
    }

    public static class SmellableConditionDecorationExtensions
    {
        public static SmellableConditionDecoration IsSmellable(this ICondition decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new SmellableConditionDecoration(decorated);
        }
    }
}
