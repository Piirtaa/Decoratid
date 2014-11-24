using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using System;
using System.Runtime.Serialization;
using Decoratid.Core.ValueOfing;

namespace Decoratid.Core.Conditional.Of
{
    /// <summary>
    /// decorate logic as a conditionOf
    /// </summary>
    [Serializable]
    public class ConditionalOfLogicDecoration<T> : DecoratedLogicBase, IConditionOf<T>
    {
        #region Ctor
        public ConditionalOfLogicDecoration(LogicOfTo<T,bool?> decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected ConditionalOfLogicDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IConditionOf
        public bool? Evaluate(T context)
        {
            LogicOfTo<T,bool?> logic = (LogicOfTo<T,bool?>)this.Decorated;
            Condition.Requires(logic).IsNotNull();
            var rv = logic.Perform(context) as LogicOfTo<T, bool?>; //note we don't bias the logic
            return rv.Result;
        }
        #endregion

        #region Methods
        public override ILogic Perform(object context = null)
        {
            return Decorated.Perform(context);
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new ConditionalOfLogicDecoration<T>((LogicOfTo<T,bool?>)thing );
        }
        #endregion
    }

    public static class ConditionalOfLogicDecorationExtensions
    {
        /// <summary>
        /// converts logic to a condition
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static ConditionalOfLogicDecoration<T> IsConditionOf<T>(this LogicOfTo<T, bool?> decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ConditionalOfLogicDecoration<T>(decorated);
        }

    }
}
