using Decoratid.Core.Decorating;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.TokenParsing
{

    public interface ITokenDecoration<T> : IToken<T>, IDecorationOf<IToken<T>> { }

    /// <summary>
    /// base class implementation of a IToken decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class TokenDecorationBase<T> : DecorationOfBase<IToken<T>>, ITokenDecoration<T>
    {
        #region Ctor
        public TokenDecorationBase(IToken<T> decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected TokenDecorationBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public virtual IToken<T> PriorToken { get { return this.Decorated.PriorToken; } set { this.Decorated.PriorToken = value; } }
        public virtual T[] TokenData { get { return this.Decorated.TokenData; } }
        public override IToken<T> This
        {
            get { return this; }
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<IToken<T>> ApplyThisDecorationTo(IToken<T> thing)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
