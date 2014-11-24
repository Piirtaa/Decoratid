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

namespace Decoratid.Core.Conditional
{
    /// <summary>
    /// A container that implements ICondition using a strategy (func of bool?)
    /// </summary>
    [Serializable]
    public sealed class StrategizedCondition : ICondition, ICloneableCondition, ISerializable
    {
        #region Ctor
        public StrategizedCondition(LogicTo<bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            this.ConditionStrategy = conditionStrategy;
        }
        public StrategizedCondition(Func<bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            this.ConditionStrategy = conditionStrategy.MakeLogicTo();
        }
        #endregion

        #region Static Fluent Methods
        public static StrategizedCondition New(LogicTo<bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new StrategizedCondition(conditionStrategy);
        }
        public static StrategizedCondition New(Func<bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new StrategizedCondition(conditionStrategy);
        }
        #endregion

        #region ISerializable
        protected StrategizedCondition(SerializationInfo info, StreamingContext context)
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
            var logic = this.ConditionStrategy.Perform() as LogicTo<bool?>; //note we don't bias the logic
            return logic.Result;
        }
        public ICondition Clone()
        {
            return new StrategizedCondition(this.ConditionStrategy);
        }
        #endregion

    }
    


}
