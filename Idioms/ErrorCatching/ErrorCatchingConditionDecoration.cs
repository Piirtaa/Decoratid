using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.ErrorCatching
{
    /// <summary>
    /// Traps all errors that are thrown from the condition evaluation
    /// </summary>
    [Serializable]
    public class ErrorCatchingConditionDecoration : DecoratedConditionBase, IErrorCatching
    {
        #region Ctor
        public ErrorCatchingConditionDecoration(ICondition decorated)
            : base(decorated)
        {
        }
        #endregion
        
        #region ISerializable
        protected ErrorCatchingConditionDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Methods
        [DebuggerStepThrough]
        public override bool? Evaluate()
        {
            try
            {
                return Decorated.Evaluate();
            }
            catch
            {
            }
            return false;
        }
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            return new ErrorCatchingConditionDecoration(thing);
        }
        #endregion
    }

    public static class ErrorCatchingConditionDecorationExtensions
    {
        public static ErrorCatchingConditionDecoration Traps(this ICondition decorated)
        {
            return new ErrorCatchingConditionDecoration(decorated);
        }
    }
}
