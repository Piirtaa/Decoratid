using Decoratid.Core.Decorating;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Expiring
{


    public interface IWindowableDecoration : IWindowable, IDecorationOf<IWindowable> { }

    /// <summary>
    /// base class implementation of a IWindowable decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class WindowableDecorationBase : DecorationOfBase<IWindowable>, IWindowable
    {
        #region Ctor
        public WindowableDecorationBase(IWindowable decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected WindowableDecorationBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public virtual bool IsInWindow(DateTime dt)
        {
            return base.Decorated.IsInWindow(dt);
        }
        public override IWindowable This
        {
            get { return this; }
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<IWindowable> ApplyThisDecorationTo(IWindowable thing)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
