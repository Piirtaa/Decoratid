using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Extensions;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Eventing
{
    public interface IEventingLogic : IDecoratedLogic
    {
        event EventHandler<EventArgOf<ILogic>> Performed;
    }

    [Serializable]
    public class EventingLogicDecoration : DecoratedLogicBase, IEventingLogic
    {
        #region Ctor
        public EventingLogicDecoration(ILogic decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected EventingLogicDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IEventingLogic
        public event EventHandler<EventArgOf<ILogic>> Performed;
        #endregion

        #region Methods
        public override void Perform()
        {
            try
            {
                this.Decorated.Perform();
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Performed.BuildAndFireEventArgs(this);
            }
            
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new EventingLogicDecoration(thing);
        }
        #endregion
    }

    public static class EventingLogicDecorationExtensions
    {
        public static EventingLogicDecoration Eventing(this ILogic decorated)
        {
            return new EventingLogicDecoration(decorated);
        }
    }
}
