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
using Decoratid.Idioms.TokenParsing.HasTokenizerId;

namespace Decoratid.Idioms.TokenParsing.HasPredecessor
{
    /// <summary>
    /// a token that tracks the prior token's tokenizer id
    /// </summary>
    public interface IHasPriorTokenizerIdToken<T> : IToken<T>
    {
        string PriorTokenizerId { get; }
    }

    /// <summary>
    /// a decoration that tracks the prior token's tokenizer id
    /// </summary>
    [Serializable]
    public class HasPriorTokenizerIdTokenDecoration<T> : TokenDecorationBase<T>, IHasPriorTokenizerIdToken<T>
    {
        #region Ctor
        public HasPriorTokenizerIdTokenDecoration(IToken<T> decorated, string priorTokenizerId)
            : base(decorated)
        {
            Condition.Requires(priorTokenizerId).IsNotNullOrEmpty();
            this.PriorTokenizerId = priorTokenizerId;
        }
        #endregion

        #region Fluent Static
        public static HasPriorTokenizerIdTokenDecoration<T> New(IToken<T> decorated, string priorTokenizerId)
        {
            return new HasPriorTokenizerIdTokenDecoration<T>(decorated, priorTokenizerId);
        }
        #endregion

        #region ISerializable
        protected HasPriorTokenizerIdTokenDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Overrides
        public string PriorTokenizerId { get; private set; }
        public override IDecorationOf<IToken<T>> ApplyThisDecorationTo(IToken<T> thing)
        {
            return new HasTokenizerIdTokenDecoration<T>(thing, this.PriorTokenizerId);
        }
        #endregion
    }

    public static class HasPriorTokenizerIdTokenDecorationExtensions
    {
        public static HasPriorTokenizerIdTokenDecoration<T> HasPriorTokenizerId<T>(this IToken<T> thing, string priorTokenizerId)
        {
            Condition.Requires(thing).IsNotNull();
            return new HasPriorTokenizerIdTokenDecoration<T>(thing, priorTokenizerId);
        }
        public static string GetPriorTokenizerId<T>(this IToken<T> token)
        {
            Condition.Requires(token).IsNotNull();
            var tokenizer = token.GetFace<IHasPriorTokenizerIdToken<T>>();
            return tokenizer.With(x => x.PriorTokenizerId);
        }
    }
}
