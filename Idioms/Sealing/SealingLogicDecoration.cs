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

namespace Decoratid.Idioms.Sealing
{
    /// <summary>
    /// prevents further decoration
    /// </summary>
    [Serializable]
    public class SealingLogicDecoration : DecoratedLogicBase
    {
        #region Ctor
        public SealingLogicDecoration(ILogic decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected SealingLogicDecoration(SerializationInfo info, StreamingContext context)
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
            Decorated.Perform();
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new SealingLogicDecoration(thing);
        }
        #endregion
    }

    public static class SealingLogicDecorationExtensions
    {
        /// <summary>
        /// prevents further decoration
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static SealingLogicDecoration Seal(ILogic decorated)
        {
            return new SealingLogicDecoration(decorated);
        }
    }
}
