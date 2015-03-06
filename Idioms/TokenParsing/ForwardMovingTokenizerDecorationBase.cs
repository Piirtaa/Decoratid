using Decoratid.Core;
using Decoratid.Core.Decorating;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.TokenParsing
{

    public interface ITokenizerDecoration<T> : IForwardMovingTokenizer<T>, IDecorationOf<IForwardMovingTokenizer<T>> { }

    /// <summary>
    /// base class implementation of a tokenizer decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class ForwardMovingTokenizerDecorationBase<T> : DecorationOfBase<IForwardMovingTokenizer<T>>, ITokenizerDecoration<T>
    {
        #region Ctor
        public ForwardMovingTokenizerDecorationBase(IForwardMovingTokenizer<T> decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected ForwardMovingTokenizerDecorationBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public virtual bool Parse(T[] rawData, int currentPosition, object state, IToken<T> currentToken,
    out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            return this.Decorated.Parse(rawData, currentPosition, state, currentToken, out newPosition, out newToken, out newParser);
        }
        public virtual IToken<T> DecorateToken(IToken<T> token, Arg arg)
        {
            return token;
        }
        public override IForwardMovingTokenizer<T> This
        {
            get { return this; }
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
