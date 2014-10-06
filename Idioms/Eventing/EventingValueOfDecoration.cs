using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Extensions;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Eventing
{

    public interface IEventingValueOf<T> : IDecoratedValueOf<T>
    {
        event EventHandler<EventArgOf<IValueOf<T>>> Evaluated;
    }

    [Serializable]
    public class EventingValueOfDecoration<T> : DecoratedValueOfBase<T>, IEventingValueOf<T>
    {
        #region Ctor
        public EventingValueOfDecoration(IValueOf<T> decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected EventingValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion


        #region IEventingValueOf
        public event EventHandler<EventArgOf<IValueOf<T>>> Evaluated;

        #endregion

        #region Methods
        public override T GetValue()
        {
            T rv;
            try
            {
                rv = this.Decorated.GetValue();
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Evaluated.BuildAndFireEventArgs(this);
            }

            return rv;
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new EventingValueOfDecoration<T>(thing);
        }
        #endregion
    }

    public static class EventingValueOfDecorationExtensions
    {
        public static EventingValueOfDecoration<T> Eventing<T>(this IValueOf<T> decorated)
        {
            return new EventingValueOfDecoration<T>(decorated);
        }
    }
}
