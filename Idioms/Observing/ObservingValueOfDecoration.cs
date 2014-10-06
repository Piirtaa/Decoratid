using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Observing
{

    /// <summary>
    /// observes and does something with the valueof but doesn't change it
    /// </summary>
    public interface IObservingValueOf<T> : IDecoratedValueOf<T>
    {
        LogicOf<IValueOf<T>> PreObservation { get; }
        LogicOf<IValueOf<T>> PostObservation { get; }
    }

    /// <summary>
    /// prevents further decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ObservingValueOfDecoration<T> : DecoratedValueOfBase<T>, IObservingValueOf<T>
    {
        #region Ctor
        public ObservingValueOfDecoration(IValueOf<T> decorated, LogicOf<IValueOf<T>> preObservation,
            LogicOf<IValueOf<T>> postObservation)
            : base(decorated)
        {
            this.PostObservation = postObservation;
            this.PreObservation = preObservation;
        }
        #endregion

        #region ISerializable
        protected ObservingValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion


        #region IObservingValueOf
        public LogicOf<IValueOf<T>> PreObservation { get; private set; }
        public LogicOf<IValueOf<T>> PostObservation { get; private set; }

        #endregion
        #region Methods
        public override T GetValue()
        {
            if (this.PreObservation != null)
                this.PreObservation.CloneAndPerform(this.Decorated.AsNaturalValue());

            var rv = this.Decorated.GetValue();

            if (this.PostObservation != null)
                this.PostObservation.CloneAndPerform(this.Decorated.AsNaturalValue());

            return rv;
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new ObservingValueOfDecoration<T>(thing, this.PreObservation, this.PostObservation);
        }
        #endregion
    }

    public static class ObservingValueOfDecorationExtensions
    {
        /// <summary>
        /// prevents further decoration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static ObservingValueOfDecoration<T> Observe<T>(this IValueOf<T> decorated, LogicOf<IValueOf<T>> preObservation,
            LogicOf<IValueOf<T>> postObservation)
        {
            return new ObservingValueOfDecoration<T>(decorated, preObservation, postObservation);
        }
    }
}
