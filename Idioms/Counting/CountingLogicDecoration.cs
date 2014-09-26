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
    public class CountingLogicDecoration : DecoratedLogicBase, IHasCounter
    {
        #region Ctor
        public CountingLogicDecoration(ILogic decorated)
            : base(decorated)
        {
            Counter = new Counter();
        }
        #endregion

        #region ISerializable
        protected CountingLogicDecoration(SerializationInfo info, StreamingContext context)
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
        public override void Perform()
        {
            this.Counter.Increment();
            Decorated.Perform();
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new CountingLogicDecoration(thing);
        }
        #endregion
    }

    public static class CountingLogicDecorationExtensions
    {
        /// <summary>
        /// Adds a Counter to track number of times evaluation has been invoked
        /// </summary>
        public static CountingLogicDecoration Counted(ILogic decorated)
        {
            return new CountingLogicDecoration(decorated);
        }
    }
}
