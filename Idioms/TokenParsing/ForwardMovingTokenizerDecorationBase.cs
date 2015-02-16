using Decoratid.Core.Decorating;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.TokenParsing
{

    public interface ITokenizerDecoration : IForwardMovingTokenizer, IDecorationOf<IForwardMovingTokenizer> { }

    /// <summary>
    /// base class implementation of a tokenizer decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class ForwardMovingTokenizerDecorationBase : DecorationOfBase<IForwardMovingTokenizer>, ITokenizerDecoration
    {
        #region Ctor
        public ForwardMovingTokenizerDecorationBase(IForwardMovingTokenizer decorated)
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
        public virtual bool Parse(string text, int currentPosition, object state, IToken currentToken,
    out int newPosition, out IToken newToken, out IForwardMovingTokenizer newParser)
        {
            return this.Decorated.Parse(text, currentPosition, state, currentToken, out newPosition, out newToken, out newParser);
        }
        public override IForwardMovingTokenizer This
        {
            get { return this; }
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<IForwardMovingTokenizer> ApplyThisDecorationTo(IForwardMovingTokenizer thing)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
