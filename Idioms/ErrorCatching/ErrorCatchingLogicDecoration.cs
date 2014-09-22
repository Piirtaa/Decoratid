using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Core;

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
        public static ErrorCatchingLogicDecoration WithErrorCatching(ILogic decorated)
        {
            return new ErrorCatchingLogicDecoration(decorated);
        }
    }
}
