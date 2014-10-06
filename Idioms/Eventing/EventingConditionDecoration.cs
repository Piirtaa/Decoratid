using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Extensions;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Eventing
{
    public interface IEventingCondition : IDecoratedCondition
    {
        event EventHandler<EventArgOf<ICondition>> Evaluated;
    }

    [Serializable]
    public class EventingConditionDecoration : DecoratedConditionBase, IEventingCondition
    {
        #region Ctor
        public EventingConditionDecoration(ICondition decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected EventingConditionDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IEventingCondition
        public event EventHandler<EventArgOf<ICondition>> Evaluated;
        #endregion

        #region Methods
        public override bool? Evaluate()
        {
            bool? rv;
            try
            {
                rv = this.Decorated.Evaluate();
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
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            return new EventingConditionDecoration(thing);
        }
        #endregion
    }

    public static class EventingConditionDecorationExtensions
    {
        public static EventingConditionDecoration Eventing(this ICondition decorated)
        {
            return new EventingConditionDecoration(decorated);
        }
    }
}
