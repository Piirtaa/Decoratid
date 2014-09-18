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

namespace Decoratid.Idioms.Core.Conditional
{
    /// <summary>
    /// A container that implements ICondition using a strategy (func of bool?)
    /// </summary>
    [Serializable]
    public sealed class NaturalStrategizedCondition : ICondition, ICloneableCondition, ISerializable
    {
        #region Ctor
        public NaturalStrategizedCondition(LogicTo<bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            this.ConditionStrategy = conditionStrategy;
        }
        public NaturalStrategizedCondition(Func<bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            this.ConditionStrategy = conditionStrategy.MakeLogicTo();
        }
        #endregion

        #region Static Fluent Methods
        public static NaturalStrategizedCondition New(LogicTo<bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new NaturalStrategizedCondition(conditionStrategy);
        }
        public static NaturalStrategizedCondition New(Func<bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new NaturalStrategizedCondition(conditionStrategy);
        }
        #endregion

        #region ISerializable
        protected NaturalStrategizedCondition(SerializationInfo info, StreamingContext context)
        {
            this.ConditionStrategy = (LogicTo<bool?>)info.GetValue("_ConditionStrategy", typeof(LogicTo<bool?>));
        }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_ConditionStrategy", this.ConditionStrategy);
        }
        #endregion

        #region Properties
        private LogicTo<bool?> ConditionStrategy { get; set; }
        #endregion

        #region Methods
        public bool? Evaluate()
        {
            Condition.Requires(this.ConditionStrategy).IsNotNull();
            return this.ConditionStrategy.CloneAndPerform();
        }
        public ICondition Clone()
        {
            return new NaturalStrategizedCondition(this.ConditionStrategy);
        }
        #endregion

    }
    


}
