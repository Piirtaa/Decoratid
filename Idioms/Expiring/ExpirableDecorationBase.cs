using Decoratid.Core.Decorating;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Expiring
{
    public interface IExpireableDecoration : IExpirable, IDecorationOf<IExpirable> { }

    /// <summary>
    /// base class implementation of a IExpirable decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class ExpirableDecorationBase : DecorationOfBase<IExpirable>, IExpirable
    {
        #region Ctor
        public ExpirableDecorationBase(IExpirable decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected ExpirableDecorationBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public virtual bool IsExpired()
        {
            return base.Decorated.IsExpired();
        }
        public override IExpirable This
        {
            get { return this; }
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<IExpirable> ApplyThisDecorationTo(IExpirable thing)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
