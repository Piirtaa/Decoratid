using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Core.Conditional
{
    /// <summary>
    /// decorate logic as a condition
    /// </summary>
    [Serializable]
    public class ConditionalLogicDecoration : DecoratedLogicBase, ICondition
    {
        #region Ctor
        public ConditionalLogicDecoration(ILogicTo<bool?> decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected ConditionalLogicDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region ICondition
        public bool? Evaluate()
        {
            ILogicTo<bool?> logic = (ILogicTo<bool?>)this.Decorated;
            Condition.Requires(logic).IsNotNull();
            var logicResults = logic.Perform() as ILogicTo<bool?>; //note we don't bias the logic
            return logicResults.Result;
        }
        #endregion

        #region Methods
        public override ILogic Perform(object context = null)
        {
            return Decorated.Perform(context);
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new ConditionalLogicDecoration((ILogicTo<bool?>)thing );
        }
        #endregion
    }

    public static class ConditionalLogicDecorationExtensions
    {
        /// <summary>
        /// converts logic to a condition
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static ConditionalLogicDecoration IsCondition(this ILogicTo<bool?> decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ConditionalLogicDecoration(decorated);
        }

    }
}
