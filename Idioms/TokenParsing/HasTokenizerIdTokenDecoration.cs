using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core;
using System.Runtime.Serialization;
using Decoratid.Core.Decorating;

namespace Decoratid.Idioms.TokenParsing
{
    /// <summary>
    /// a token that tracks who tokenized
    /// </summary>
    public interface IHasTokenizerId : IToken
    {
        string TokenizerId { get; }
    }

    /// <summary>
    /// decorates with who tokenized
    /// </summary>
    [Serializable]
    public class HasTokenizerIdTokenDecoration : TokenDecorationBase, IHasTokenizerId
    {
        #region Ctor
        public HasTokenizerIdTokenDecoration(IToken decorated, string tokenizerId)
            : base(decorated)
        {
            Condition.Requires(tokenizerId).IsNotNullOrEmpty();
            this.TokenizerId = tokenizerId;
        }
        #endregion

        #region Fluent Static
        public static HasTokenizerIdTokenDecoration New(IToken decorated, string tokenizerId)
        {
            return new HasTokenizerIdTokenDecoration(decorated, tokenizerId);
        }
        #endregion

        #region ISerializable
        protected HasTokenizerIdTokenDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public string TokenizerId { get; private set; }
        public override IDecorationOf<IToken> ApplyThisDecorationTo(IToken thing)
        {
            return new HasTokenizerIdTokenDecoration(thing, this.TokenizerId);
        }
        #endregion
    }

    public static class HasTokenizerIdTokenDecorationExtensions
    {
        public static HasTokenizerIdTokenDecoration HasTokenizerId(this IToken thing, string tokenizerId)
        {
            Condition.Requires(thing).IsNotNull();
            return new HasTokenizerIdTokenDecoration(thing, tokenizerId);
        }
    }
}
