using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;

namespace Decoratid.Core.Conditional.Of
{
    /// <summary>
    /// A container that implements IConditionOf using a strategy operating on a contextual object of T.
    /// </summary>
    /// <remarks>
    /// How is this any different from the ConditionalOfLogicDecoration?  It doesn't expose the ILogic roots,
    /// and has a tighter, more-defined implementation (eg. it does a CloneAndPerform() under the hood).
    /// </remarks>
    [Serializable]
    public sealed class StrategizedConditionOf<T> : IConditionOf<T>, ISerializable
    {
        #region Ctor
        public StrategizedConditionOf(LogicOfTo<T, bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            this.ConditionStrategy = conditionStrategy;
        }
        /// <summary>
        /// uses delegates and not LogicWrappers
        /// </summary>
        /// <param name="conditionStrategy"></param>
        public StrategizedConditionOf(Func<T, bool?> conditionStrategy)
            : this(conditionStrategy.MakeLogicOfTo())
        {
        }
        #endregion

        #region Static Fluent Methods
        public static StrategizedConditionOf<T> New(LogicOfTo<T, bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new StrategizedConditionOf<T>(conditionStrategy);
        }
        public static StrategizedConditionOf<T> New(Func<T, bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new StrategizedConditionOf<T>(conditionStrategy);
        }
        #endregion

        #region ISerializable
        protected StrategizedConditionOf(SerializationInfo info, StreamingContext context)
        {
            this.ConditionStrategy = (LogicOfTo<T, bool?>)info.GetValue("_ConditionStrategy", typeof(LogicOfTo<T, bool?>));
        }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_ConditionStrategy", this.ConditionStrategy);
        }
        #endregion

        #region Properties
        protected LogicOfTo<T, bool?> ConditionStrategy { get; set; }
        #endregion

        #region Methods
        public bool? Evaluate(T context)
        {
            Condition.Requires(this.ConditionStrategy).IsNotNull();
            var logic = this.ConditionStrategy.Perform(context) as LogicOfTo<T, bool?>; //note we don't bias the logic
            return logic.Result;
        }
        #endregion
    }

    public static class StrategizedConditionOfExtensions
    {
        public static StrategizedConditionOf<T> BuildConditionOf<T>(this LogicOfTo<T, bool?> conditionStrategy)
        {
            return new StrategizedConditionOf<T>(conditionStrategy);
        }
    }
}
