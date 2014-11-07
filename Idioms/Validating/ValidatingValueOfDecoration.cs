using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Validating
{
    /// <summary>
    /// kacks on evaluation if the validating condition isn't true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    [Serializable]
    public class ValidatingValueOfDecoration<T> : DecoratedValueOfBase<T>, IHasValidator
    {
        #region Ctor
        public ValidatingValueOfDecoration(IValueOf<T> decorated, ICondition isValidCondition)
            : base(decorated)
        {
            Condition.Requires(isValidCondition).IsNotNull();
            this.IsValidCondition = isValidCondition;
        }
        #endregion

        #region ISerializable
        protected ValidatingValueOfDecoration(SerializationInfo info, StreamingContext context)
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
        public override T GetValue()
        {
            var condVal = this.IsValidCondition.Evaluate();
            if (!condVal.GetValueOrDefault())
                throw new InvalidOperationException("Condition not ready");

            return Decorated.GetValue();
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new ValidatingValueOfDecoration<T>(thing, this.IsValidCondition);
        }
        #endregion
    }

    public static class ValidatingValueOfDecorationExtensions
    {
        public static ValidatingValueOfDecoration<T> KackUnless<T>(this IValueOf<T> decorated, ICondition validatingCondition)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ValidatingValueOfDecoration<T>(decorated, validatingCondition);
        }
    }
}
