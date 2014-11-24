using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Validating
{
    /// <summary>
    /// kacks on evaluation if the validating condition isn't true
    /// </summary>
    /// 
    [Serializable]
    public class ValidatingLogicDecoration : DecoratedLogicBase, IHasValidator
    {
        #region Ctor
        public ValidatingLogicDecoration(ILogic decorated, ICondition isValidCondition)
            : base(decorated)
        {
            Condition.Requires(isValidCondition).IsNotNull();
            this.IsValidCondition = isValidCondition;
        }
        #endregion

        #region ISerializable
        protected ValidatingLogicDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.IsValidCondition = (ICondition)info.GetValue("IsValidCondition", typeof(ICondition));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsValidCondition", this.IsValidCondition); ;
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public ICondition IsValidCondition { get; private set; }
        #endregion

        #region Methods
        public override ILogic Perform(object context = null)
        {
            var condVal = this.IsValidCondition.Evaluate();
            if (!condVal.GetValueOrDefault())
                throw new InvalidOperationException("Condition not ready");

            var rv = Decorated.Perform(context);
            return rv;
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new ValidatingLogicDecoration(thing, this.IsValidCondition);
        }
        #endregion
    }

    public static class ValidatingLogicDecorationExtensions
    {
        public static ValidatingLogicDecoration KackUnless(this ILogic decorated, ICondition validatingCondition)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ValidatingLogicDecoration(decorated, validatingCondition);
        }
    }
}
