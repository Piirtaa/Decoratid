using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Observing
{
    /// <summary>
    /// observes and does something with the logic but doesn't change it
    /// </summary>
    public interface IObservingLogic : IDecoratedLogic
    {
        LogicOf<ILogic> PreObservation { get; }
        LogicOf<ILogic> PostObservation { get; }
    }

    /// <summary>
    /// adds observing logic
    /// </summary>
    [Serializable]
    public class ObservingLogicDecoration : DecoratedLogicBase, IObservingLogic
    {
        #region Ctor
        public ObservingLogicDecoration(ILogic decorated, LogicOf<ILogic> preObservation,
            LogicOf<ILogic> postObservation)
            : base(decorated)
        {
            this.PostObservation = postObservation;
            this.PreObservation = preObservation;
        }
        #endregion

        #region ISerializable
        protected ObservingLogicDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.PostObservation = (LogicOf<ILogic>)info.GetValue("PostObservation", typeof(LogicOf<ILogic>));
            this.PreObservation = (LogicOf<ILogic>)info.GetValue("PreObservation", typeof(LogicOf<ILogic>));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PostObservation", this.PostObservation);
            info.AddValue("PreObservation", this.PreObservation);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion


        #region IObservingLogic
        public LogicOf<ILogic> PreObservation { get; private set; }
        public LogicOf<ILogic> PostObservation { get; private set; }
        #endregion

        #region Methods
        public override ILogic Perform(object context = null)
        {
            if (this.PreObservation != null)
                this.PreObservation.Perform(this.Decorated);

            ILogic rv = null;
            rv = Decorated.Perform(context);

            if (this.PostObservation != null)
                this.PostObservation.Perform(this.Decorated);

            return rv;
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new ObservingLogicDecoration(thing, this.PreObservation, this.PostObservation);
        }
        #endregion
    }

    public static class ObservingLogicDecorationExtensions
    {
        /// <summary>
        /// adds observing logic
        /// </summary>
        public static ObservingLogicDecoration Observe(this ILogic decorated, LogicOf<ILogic> preObservation,
            LogicOf<ILogic> postObservation)
        {
            return new ObservingLogicDecoration(decorated, preObservation, postObservation);
        }
    }
}
