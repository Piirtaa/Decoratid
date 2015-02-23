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

namespace Decoratid.Idioms.TokenParsing.HasTokenizerId
{
    /// <summary>
    /// a token that tracks who tokenized
    /// </summary>
    public interface IHasTokenizerId<T> : IToken<T>
    {
        string TokenizerId { get; }
    }

    /// <summary>
    /// decorates with who tokenized
    /// </summary>
    [Serializable]
    public class HasTokenizerIdTokenDecoration<T> : TokenDecorationBase<T>, IHasTokenizerId<T>
    {
        #region Ctor
        public HasTokenizerIdTokenDecoration(IToken<T> decorated, string tokenizerId)
            : base(decorated)
        {
            Condition.Requires(tokenizerId).IsNotNullOrEmpty();
            this.TokenizerId = tokenizerId;
        }
        #endregion

        #region Fluent Static
        public static HasTokenizerIdTokenDecoration<T> New(IToken<T> decorated, string tokenizerId)
        {
            return new HasTokenizerIdTokenDecoration<T>(decorated, tokenizerId);
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
        public override IDecorationOf<IToken<T>> ApplyThisDecorationTo(IToken<T> thing)
        {
            return new HasTokenizerIdTokenDecoration<T>(thing, this.TokenizerId);
        }
        #endregion
    }

    public static class HasTokenizerIdTokenDecorationExtensions
    {
        public static HasTokenizerIdTokenDecoration<T> HasTokenizerId<T>(this IToken<T> thing, string tokenizerId)
        {
            Condition.Requires(thing).IsNotNull();
            return new HasTokenizerIdTokenDecoration<T>(thing, tokenizerId);
        }
    }
}
