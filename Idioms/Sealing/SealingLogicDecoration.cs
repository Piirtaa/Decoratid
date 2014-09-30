using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Sealing
{
    /// <summary>
    /// prevents further decoration
    /// </summary>
    [Serializable]
    public class SealingLogicDecoration : DecoratedLogicBase, ISealedDecoration
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
        public static SealingLogicDecoration Seal(this ILogic decorated)
        {
            return new SealingLogicDecoration(decorated);
        }
    }
}
