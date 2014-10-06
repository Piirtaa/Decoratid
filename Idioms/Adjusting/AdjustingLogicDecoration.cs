using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Adjusting
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
    /// prevents further decoration
    /// </summary>
    [Serializable]
    public class AdjustingLogicDecoration : DecoratedLogicBase, IObservingLogic
    {
        #region Ctor
        public AdjustingLogicDecoration(ILogic decorated, LogicOf<ILogic> preObservation,
            LogicOf<ILogic> postObservation)
            : base(decorated)
        {
            this.PostObservation = postObservation;
            this.PreObservation = preObservation;
        }
        #endregion

        #region ISerializable
        protected AdjustingLogicDecoration(SerializationInfo info, StreamingContext context)
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
        public override void Perform()
        {
            if (this.PreObservation != null)
                this.PreObservation.CloneAndPerform(this.Decorated.AsNaturalValue());

            Decorated.Perform();

            if (this.PostObservation != null)
                this.PostObservation.CloneAndPerform(this.Decorated.AsNaturalValue());
            
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new AdjustingLogicDecoration(thing, this.PreObservation, this.PostObservation);
        }
        #endregion
    }

    public static class ObservingLogicDecorationExtensions
    {
        /// <summary>
        /// prevents further decoration
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static AdjustingLogicDecoration Observe(this ILogic decorated, LogicOf<ILogic> preObservation,
            LogicOf<ILogic> postObservation)
        {
            return new AdjustingLogicDecoration(decorated, preObservation, postObservation);
        }
    }
}
