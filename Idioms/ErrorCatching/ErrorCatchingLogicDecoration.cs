﻿using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using System;
using System.Diagnostics;
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
        [DebuggerStepThrough]
        public override ILogic Perform(object context = null)
        {
            ILogic rv = null;
            try
            {
                rv = Decorated.Perform(context);
            }
            catch
            {
            }
            return rv;
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new ErrorCatchingLogicDecoration(thing);
        }
        #endregion
    }

    public static class ErrorCatchingLogicDecorationExtensions
    {
        public static ErrorCatchingLogicDecoration Traps(this ILogic decorated)
        {
            return new ErrorCatchingLogicDecoration(decorated);
        }
    }
}
