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
    /// a token that tracks the prior token's tokenizer id
    /// </summary>
    public interface IHasPriorTokenizerIdToken : IToken
    {
        string PriorTokenizerId { get; }
    }

    /// <summary>
    /// a decoration that tracks the prior token's tokenizer id
    /// </summary>
    [Serializable]
    public class HasPriorTokenizerIdTokenDecoration : TokenDecorationBase, IHasPriorTokenizerIdToken
    {
        #region Ctor
        public HasPriorTokenizerIdTokenDecoration(IToken decorated, string priorTokenizerId)
            : base(decorated)
        {
            Condition.Requires(priorTokenizerId).IsNotNullOrEmpty();
            this.PriorTokenizerId = priorTokenizerId;
        }
        #endregion

        #region Fluent Static
        public static HasPriorTokenizerIdTokenDecoration New(IToken decorated, string priorTokenizerId)
        {
            return new HasPriorTokenizerIdTokenDecoration(decorated, priorTokenizerId);
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
        public override IDecorationOf<IToken> ApplyThisDecorationTo(IToken thing)
        {
            return new HasTokenizerIdTokenDecoration(thing, this.PriorTokenizerId);
        }
        #endregion
    }

    public static class HasPriorTokenizerIdTokenDecorationExtensions
    {
        public static HasPriorTokenizerIdTokenDecoration HasPriorTokenizerId(this IToken thing, string priorTokenizerId)
        {
            Condition.Requires(thing).IsNotNull();
            return new HasPriorTokenizerIdTokenDecoration(thing, priorTokenizerId);
        }
    }
}
