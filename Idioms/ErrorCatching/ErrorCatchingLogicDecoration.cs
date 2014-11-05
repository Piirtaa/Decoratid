using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.ErrorCatching
{
    /// <summary>
    /// Traps all errors that are thrown from the logic
    /// </summary>
    [Serializable]
    public class ErrorCatchingLogicDecoration : DecoratedLogicBase, IErrorCatching
    {
        #region Ctor
        public ErrorCatchingLogicDecoration(ILogic decorated)
            : base(decorated)
        {
        }
        #endregion
        
        #region ISerializable
        protected ErrorCatchingLogicDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Methods
        public override void Perform()
        {
            try
            {
                Decorated.Perform();
            }
            catch
            {
            }
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new ErrorCatchingLogicDecoration(thing);
        }
        #endregion
    }

    public static class ErrorCatchingLogicDecorationExtensions
    {
        public static ErrorCatchingLogicDecoration Trap(this ILogic decorated)
        {
            return new ErrorCatchingLogicDecoration(decorated);
        }
    }
}
