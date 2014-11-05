using Decoratid.Core.Decorating;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.ErrorCatching
{
    /// <summary>
    /// Traps all errors that are thrown from the logic
    /// </summary>
    [Serializable]
    public class ErrorCatchingValueOfDecoration<T> : DecoratedValueOfBase<T>, IErrorCatching
    {
        #region Ctor
        public ErrorCatchingValueOfDecoration(IValueOf<T> decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected ErrorCatchingValueOfDecoration(SerializationInfo info, StreamingContext context)
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
            try
            {
                return Decorated.GetValue();
            }
            catch
            {
            }
            return default(T);
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new ErrorCatchingValueOfDecoration<T>(thing);
        }
        #endregion
    }

    public static class ErrorCatchingValueOfDecorationExtensions
    {
        public static ErrorCatchingValueOfDecoration<T> Trap<T>(this IValueOf<T> decorated)
        {
            return new ErrorCatchingValueOfDecoration<T>(decorated);
        }
    }
}
