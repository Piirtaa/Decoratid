using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness;
using Decoratid.Thingness.Idioms.Logics;
using Decoratid.Thingness.Idioms.ValuesOf;

namespace Decoratid.Thingness.Idioms.Conditions
{
    /// <summary>
    /// A container that implements ICondition using a strategy (func of bool?)
    /// </summary>
    /// 
    [Serializable]
    public class StrategizedCondition : ICondition, ICloneableCondition
    {
        #region Ctor
        protected StrategizedCondition() { }
        /// <summary>
        /// ctor, providing a strategy
        /// </summary>
        /// <param name="conditionStrategy"></param>
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

        #region Properties
        protected LogicTo<bool?> ConditionStrategy { get; set; }
        #endregion

        #region Methods
        public virtual bool? Evaluate()
        {
            Condition.Requires(this.ConditionStrategy).IsNotNull();
            return this.ConditionStrategy.CloneAndPerform();
        }
        public virtual ICondition Clone()
        {
            return new StrategizedCondition(this.ConditionStrategy);
        }
        #endregion

        #region Static Fluent Methods
        public static ICondition New(LogicTo<bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new StrategizedCondition(conditionStrategy);
        }
        public static ICondition New(Func<bool?> conditionStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new StrategizedCondition(conditionStrategy);
        }
        #endregion

        //#region ISerializable
        //protected StrategizedCondition(SerializationInfo info, StreamingContext context)
        //{
        //    this.ConditionStrategy = (LogicTo<bool?>)info.GetValue("_ConditionStrategy", typeof(LogicTo<bool?>));
        //}
        //public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("_ConditionStrategy", this.ConditionStrategy);
        //}
        //#endregion
    }
    /// <summary>
    /// a mutable StrategizedCondition
    /// </summary>
    /// 
    [Serializable]
    public class MutableStrategizedCondition : StrategizedCondition, IMutableCondition
    {
        #region Ctor
        protected MutableStrategizedCondition() { }
        public MutableStrategizedCondition(LogicTo<bool?> conditionStrategy, Logic mutateStrategy)
            : base(conditionStrategy)
        {
            Condition.Requires(mutateStrategy).IsNotNull();
            this.MutateStrategy = mutateStrategy;
        }
        public MutableStrategizedCondition(Func<bool?> conditionStrategy, Action mutateStrategy)
            : base(conditionStrategy.MakeLogicTo())
        {
            Condition.Requires(mutateStrategy).IsNotNull();
            this.MutateStrategy = mutateStrategy.MakeLogic();
        }
        #endregion

        #region Properties
        protected Logic MutateStrategy { get; set; }
        #endregion

        #region Methods
        public void Mutate()
        {
            if (MutateStrategy == null) { return; }

            this.MutateStrategy.Perform();
        }
        public override ICondition Clone()
        {
            return new MutableStrategizedCondition(this.ConditionStrategy, this.MutateStrategy);
        }
        #endregion

        #region Static Fluent Methods
        public static ICondition New(LogicTo<bool?> conditionStrategy, Logic mutateStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new MutableStrategizedCondition(conditionStrategy, mutateStrategy);
        }
        public static ICondition New(Func<bool?> conditionStrategy, Action mutateStrategy)
        {
            Condition.Requires(conditionStrategy).IsNotNull();
            return new MutableStrategizedCondition(conditionStrategy, mutateStrategy);
        }
        #endregion

        //#region ISerializable
        //protected MutableStrategizedCondition(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //    this.MutateStrategy = (Logic)info.GetValue("_MutateStrategy", typeof(Logic));
        //}
        //public override void GetObjectData(SerializationInfo info, StreamingContext context)
        //{ 
        //    base.GetObjectData(info, context);
        //   info.AddValue("_MutateStrategy", this.MutateStrategy);
        //}
        //#endregion
    }





}
