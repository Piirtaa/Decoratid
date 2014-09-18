using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Core.ValueOfing;

namespace Decoratid.Idioms.Core.Conditional.Of
{
    /// <summary>
    /// A container that implements IConditionOf using a strategy operating on a contextual object of T.
    /// </summary>
    [Serializable]
    public sealed class NaturalStrategizedConditionOf<T> : IConditionOf<T>, ISerializable
    {
        #region Ctor
        public NaturalStrategizedConditionOf(LogicOfTo<T, bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            this.ConditionStrategy = conditionStrategy;
        }
        /// <summary>
        /// uses delegates and not LogicWrappers
        /// </summary>
        /// <param name="conditionStrategy"></param>
        public NaturalStrategizedConditionOf(Func<T, bool?> conditionStrategy)
            : this(conditionStrategy.MakeLogicOfTo())
        {
        }
        #endregion

        #region Static Fluent Methods
        public static NaturalStrategizedConditionOf<T> New(LogicOfTo<T, bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new NaturalStrategizedConditionOf<T>(conditionStrategy);
        }
        public static NaturalStrategizedConditionOf<T> New(Func<T, bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new NaturalStrategizedConditionOf<T>(conditionStrategy);
        }
        #endregion

        #region ISerializable
        protected NaturalStrategizedConditionOf(SerializationInfo info, StreamingContext context)
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
            //clone the logic, set context, and run the logic
            var res = this.ConditionStrategy.CloneAndPerform(context.AsNaturalValue());
            return res;
        }
        #endregion
    }
}
