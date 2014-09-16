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

namespace Decoratid.Idioms.Core.Conditional.Core
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
    ///// <summary>
    ///// a mutable StrategizedCondition
    ///// </summary>
    ///// 
    //[Serializable]
    //public class MutableStrategizedCondition : StrategizedCondition, IMutableCondition
    //{
    //    #region Ctor
    //    protected MutableStrategizedCondition() { }
    //    public MutableStrategizedCondition(LogicTo<bool?> conditionStrategy, Logic mutateStrategy)
    //        : base(conditionStrategy)
    //    {
    //        Condition.Requires(mutateStrategy).IsNotNull();
    //        this.MutateStrategy = mutateStrategy;
    //    }
    //    public MutableStrategizedCondition(Func<bool?> conditionStrategy, Action mutateStrategy)
    //        : base(conditionStrategy.MakeLogicTo())
    //    {
    //        Condition.Requires(mutateStrategy).IsNotNull();
    //        this.MutateStrategy = mutateStrategy.MakeLogic();
    //    }
    //    #endregion

    //    #region Properties
    //    protected Logic MutateStrategy { get; set; }
    //    #endregion

    //    #region Methods
    //    public void Mutate()
    //    {
    //        if (MutateStrategy == null) { return; }

    //        this.MutateStrategy.Perform();
    //    }
    //    public override ICondition Clone()
    //    {
    //        return new MutableStrategizedCondition(this.ConditionStrategy, this.MutateStrategy);
    //    }
    //    #endregion

    //    #region Static Fluent Methods
    //    public static ICondition New(LogicTo<bool?> conditionStrategy, Logic mutateStrategy)
    //    {
    //        Condition.Requires(conditionStrategy).IsNotNull();
    //        return new MutableStrategizedCondition(conditionStrategy, mutateStrategy);
    //    }
    //    public static ICondition New(Func<bool?> conditionStrategy, Action mutateStrategy)
    //    {
    //        Condition.Requires(conditionStrategy).IsNotNull();
    //        return new MutableStrategizedCondition(conditionStrategy, mutateStrategy);
    //    }
    //    #endregion

    //    //#region ISerializable
    //    //protected MutableStrategizedCondition(SerializationInfo info, StreamingContext context)
    //    //    : base(info, context)
    //    //{
    //    //    this.MutateStrategy = (Logic)info.GetValue("_MutateStrategy", typeof(Logic));
    //    //}
    //    //public override void GetObjectData(SerializationInfo info, StreamingContext context)
    //    //{ 
    //    //    base.GetObjectData(info, context);
    //    //   info.AddValue("_MutateStrategy", this.MutateStrategy);
    //    //}
    //    //#endregion
    //}





}
