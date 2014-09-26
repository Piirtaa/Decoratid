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

namespace Decoratid.Idioms.Counting
{
    /// <summary>
    /// Adds a Counter to track number of times evaluation has been invoked
    /// </summary>
    [Serializable]
    public class CountingConditionDecoration : DecoratedConditionBase, IHasCounter
    {
        #region Ctor
        public CountingConditionDecoration(ICondition decorated)
            : base(decorated)
        {
            Counter = new Counter();
        }
        #endregion

        #region ISerializable
        protected CountingConditionDecoration(SerializationInfo info, StreamingContext context)
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
        public override bool? Evaluate()
        {
            this.Counter.Increment();
            return base.Evaluate();
        }
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            return new CountingConditionDecoration(thing);
        }
        #endregion
    }

    public static class CountingConditionDecorationExtensions
    {
        /// <summary>
        /// Adds a Counter to track number of times evaluation has been invoked
        /// </summary>
        public static CountingConditionDecoration Counted(ICondition decorated)
        {
            return new CountingConditionDecoration(decorated);
        }
    }
}
