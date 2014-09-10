using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Logics;
using Decoratid.Thingness.Idioms.ValuesOf;

namespace Decoratid.Thingness.Idioms.Conditions
{
    /// <summary>
    /// A container that implements ICondition using a strategy operating on a contextual object of T.
    /// </summary>
    /// 
    [Serializable]
    public class StrategizedConditionOf<T> : IConditionOf<T>
    {
        #region Ctor
        protected StrategizedConditionOf() { }
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

        #region Properties
        protected LogicOfTo<T, bool?> ConditionStrategy { get; set; }
        #endregion

        #region Methods
        public virtual bool? Evaluate(T context)
        {
            Condition.Requires(this.ConditionStrategy).IsNotNull();
            //clone the logic, set context, and run the logic
            var res = this.ConditionStrategy.CloneAndPerform(context.ValueOf());
            return res;
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

        //#region ISerializable
        //protected StrategizedConditionOf(SerializationInfo info, StreamingContext context)
        //{
        //    this.ConditionStrategy = (LogicOfTo<T, bool?>)info.GetValue("_ConditionStrategy", typeof(LogicOfTo<T, bool?>));
        //}
        //public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("_ConditionStrategy", this.ConditionStrategy);
        //}
        //#endregion
    }
}
