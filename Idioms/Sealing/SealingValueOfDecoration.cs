using Decoratid.Core.Decorating;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Sealing
{

    /// <summary>
    /// prevents further decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SealingValueOfDecoration<T> : DecoratedValueOfBase<T>, ISealedDecoration
    {
        #region Ctor
        public SealingValueOfDecoration(IValueOf<T> decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected SealingValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Methods
        public override T GetValue()
        {
            return Decorated.GetValue();
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new SealingValueOfDecoration<T>(thing);
        }
        #endregion
    }

    public static class SealingValueOfDecorationExtensions
    {
        /// <summary>
        /// prevents further decoration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static SealingValueOfDecoration<T> Seal<T>(this IValueOf<T> decorated)
        {
            return new SealingValueOfDecoration<T>(decorated);
        }
    }
}
