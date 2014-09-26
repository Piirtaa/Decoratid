using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Core.Logical;
using Decoratid.Core;
using Decoratid.Core.ValueOfing;

namespace Decoratid.Idioms.Counting
{
    /// <summary>
    /// Adds a Counter to track number of times evaluation has been invoked
    /// </summary>
    [Serializable]
    public class CountingValueOfDecoration<T> : DecoratedValueOfBase<T>, IHasCounter
    {
        #region Ctor
        public CountingValueOfDecoration(IValueOf<T> decorated)
            : base(decorated)
        {
            Counter = new Counter();
        }
        #endregion

        #region ISerializable
        protected CountingValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Counter = (Counter)info.GetValue("Counter", typeof(Counter));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Counter", this.Counter);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public Counter Counter { get; private set; }
        #endregion

        #region Methods
        public override T GetValue()
        {
            this.Counter.Increment();
            return Decorated.GetValue();
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new CountingValueOfDecoration<T>(thing);
        }
        #endregion
    }

    public static class CountingValueOfDecorationExtensions
    {
        /// <summary>
        /// Adds a Counter to track number of times evaluation has been invoked
        /// </summary>
        public static CountingValueOfDecoration<T> Counted<T>(IValueOf<T> decorated)
        {
            return new CountingValueOfDecoration<T>(decorated);
        }
    }
}
