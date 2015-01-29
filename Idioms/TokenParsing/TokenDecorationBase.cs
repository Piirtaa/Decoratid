using Decoratid.Core.Decorating;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.TokenParsing
{

    public interface ITokenDecoration : IToken, IDecorationOf<IToken> { }

    /// <summary>
    /// base class implementation of a IToken decoration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class TokenDecorationBase : DecorationOfBase<IToken>, ITokenDecoration
    {
        #region Ctor
        public TokenDecorationBase(IToken decorated)
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
        public virtual IToken PriorToken { get { return this.Decorated.PriorToken; } set { this.Decorated.PriorToken = value; } }
        public virtual string GetStringValue()
        {
            return this.Decorated.GetStringValue();
        }
        public override IToken This
        {
            get { return this; }
        }
        #endregion

        #region IDecoration
        public override IDecorationOf<IToken> ApplyThisDecorationTo(IToken thing)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
