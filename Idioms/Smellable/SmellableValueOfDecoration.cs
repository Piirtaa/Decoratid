using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;
using Decoratid.Idioms.ObjectGraphing;

namespace Decoratid.Idioms.Smellable
{

    [Serializable]
    public class SmellableValueOfDecoration<T> : DecoratedValueOfBase<T>, ISmellable
    {
        #region Ctor
        public SmellableValueOfDecoration(IValueOf<T> decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected SmellableValueOfDecoration(SerializationInfo info, StreamingContext context)
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

        #region Methods
        public override T GetValue()
        {
            this.SmellCheck();

            return Decorated.GetValue();
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new SmellableValueOfDecoration<T>(thing);
        }
        #endregion
    }

    public static class SmellableValueOfDecorationExtensions
    {
        public static SmellableValueOfDecoration<T> IsSmellable<T>(IValueOf<T> decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new SmellableValueOfDecoration<T>(decorated);
        }

    }
}
