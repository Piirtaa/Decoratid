using Decoratid.Core.Decorating;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Touching
{
    public interface ITouchableDecoration : ITouchable, IDecorationOf<ITouchable> { }

    /// <summary>
    /// base class implementation of a ITouchable decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class TouchableDecorationBase : DecorationOfBase<ITouchable>, ITouchable
    {
        #region Ctor
        public TouchableDecorationBase(ITouchable decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected TouchableDecorationBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public virtual ITouchable Touch()
        {
            base.Decorated.Touch();
            return this;
        }
        public override ITouchable This
        {
            get { return this; }
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<ITouchable> ApplyThisDecorationTo(ITouchable thing)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
