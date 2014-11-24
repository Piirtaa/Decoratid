using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Observing
{
    /// <summary>
    /// observes and does something with the condition but doesn't change it
    /// </summary>
    public interface IObservingCondition : IDecoratedCondition
    {
        LogicOf<ICondition> PreObservation { get; }
        LogicOf<ICondition> PostObservation { get; }
    }

    /// <summary>
    /// adds observing logic
    /// </summary>
    [Serializable]
    public class ObservingConditionDecoration : DecoratedConditionBase, IObservingCondition
    {
        #region Ctor
        public ObservingConditionDecoration(ICondition decorated, LogicOf<ICondition> preObservation,
            LogicOf<ICondition> postObservation)
            : base(decorated)
        {
            this.PostObservation = postObservation;
            this.PreObservation = preObservation;
        }
        #endregion

        #region ISerializable
        protected ObservingConditionDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.PostObservation = (LogicOf<ICondition>)info.GetValue("PostObservation", typeof(LogicOf<ICondition>));
            this.PreObservation = (LogicOf<ICondition>)info.GetValue("PreObservation", typeof(LogicOf<ICondition>));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PostObservation", this.PostObservation);
            info.AddValue("PreObservation", this.PreObservation);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IObservingCondition
        public LogicOf<ICondition> PreObservation { get; private set; }
        public LogicOf<ICondition> PostObservation { get; private set; }
        #endregion

        #region Methods
        public override bool? Evaluate()
        {
            if (this.PreObservation != null)
                this.PreObservation.Perform(this.Decorated);

            var rv = this.Decorated.Evaluate();

            if (this.PostObservation != null)
                this.PostObservation.Perform(this.Decorated);

            return rv;
        }
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            return new ObservingConditionDecoration(thing, this.PreObservation, this.PostObservation);
        }
        #endregion
    }

    public static class ObservingConditionDecorationExtensions
    {

        /// <summary>
        /// adds observing logic
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="preObservation"></param>
        /// <param name="postObservation"></param>
        /// <returns></returns>
        public static ObservingConditionDecoration Observe(this ICondition decorated, LogicOf<ICondition> preObservation,
            LogicOf<ICondition> postObservation)
        {
            return new ObservingConditionDecoration(decorated, preObservation, postObservation);
        }
    }
}
