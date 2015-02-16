using CuttingEdge.Conditions;
using Decoratid.Core;
using Decoratid.Core.Decorating;
using Decoratid.Core.Storing;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing
{
    /// <summary>
    /// tokenizer requires the stated prior tokenizer
    /// </summary>
    public interface IHasSuccessorTokenizer : IForwardMovingTokenizer
    {
        IForwardMovingTokenizer NextTokenizer { get; }
    }

    /// <summary>
    /// tokenizer requires the stated prior tokenizer
    /// </summary>
    [Serializable]
    public class HasSuccessorTokenizerDecoration : ForwardMovingTokenizerDecorationBase, IHasSuccessorTokenizer
    {
        #region Ctor
        public HasSuccessorTokenizerDecoration(IForwardMovingTokenizer decorated, IForwardMovingTokenizer nextTokenizer)
            : base(decorated)
        {
            Condition.Requires(nextTokenizer).IsNotNull();
            this.NextTokenizer = nextTokenizer;
        }
        #endregion

        #region Fluent Static
        public static HasSuccessorTokenizerDecoration New(IForwardMovingTokenizer decorated, IForwardMovingTokenizer nextTokenizer)
        {
            return new HasSuccessorTokenizerDecoration(decorated, nextTokenizer);
        }
        #endregion

        #region ISerializable
        protected HasSuccessorTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public IForwardMovingTokenizer NextTokenizer { get; private set; }

        public override bool Parse(string text, int currentPosition, object state, IToken currentToken, out int newPosition, out IToken newToken, out IForwardMovingTokenizer newParser)
        {
            var rv = base.Parse(text, currentPosition, state, currentToken, out newPosition, out newToken, out newParser);

            newParser = this.NextTokenizer;
            return rv;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer> ApplyThisDecorationTo(IForwardMovingTokenizer thing)
        {
            return new HasSuccessorTokenizerDecoration(thing, this.NextTokenizer);
        }
        #endregion
    }

    public static class HasSuccessorTokenizerDecorationExtensions
    {
        public static HasSuccessorTokenizerDecoration TransitionsTo(this IForwardMovingTokenizer decorated, IForwardMovingTokenizer tokenizer)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasSuccessorTokenizerDecoration(decorated, tokenizer);
        }
    }
}


