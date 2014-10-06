using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Adjusting
{
    /// <summary>
    /// observes and does something with the condition but doesn't change it
    /// </summary>
    public interface IObservingCondition : IDecoratedCondition
    {
        LogicOf<ICondition> PreObservation { get; }
        LogicOf<ICondition> PostObservation { get; }
    }

    [Serializable]
    public class AdjustingConditionDecoration : DecoratedConditionBase, IObservingCondition
    {
        #region Ctor
        public AdjustingConditionDecoration(ICondition decorated, LogicOf<ICondition> preObservation,
            LogicOf<ICondition> postObservation)
            : base(decorated)
        {
            this.PostObservation = postObservation;
            this.PreObservation = preObservation;
        }
        #endregion

        #region ISerializable
        protected AdjustingConditionDecoration(SerializationInfo info, StreamingContext context)
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
                this.PreObservation.CloneAndPerform(this.Decorated.AsNaturalValue());

            var rv = this.Decorated.Evaluate();

            if (this.PostObservation != null)
                this.PostObservation.CloneAndPerform(this.Decorated.AsNaturalValue());

            return rv;
        }
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            return new AdjustingConditionDecoration(thing, this.PreObservation, this.PostObservation);
        }
        #endregion
    }

    public static class ObservingConditionDecorationExtensions
    {
        /// <summary>
        /// prevents further decoration
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static AdjustingConditionDecoration Observe(this ICondition decorated, LogicOf<ICondition> preObservation,
            LogicOf<ICondition> postObservation)
        {
            return new AdjustingConditionDecoration(decorated, preObservation, postObservation);
        }
    }
}
