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

namespace Decoratid.Idioms.TokenParsing.HasSuccessor
{
    /// <summary>
    /// tokenizer requires the stated prior tokenizer
    /// </summary>
    public interface IHasSuccessorTokenizer<T> : IForwardMovingTokenizer<T>
    {
        IForwardMovingTokenizer<T> NextTokenizer { get; }
    }

    /// <summary>
    /// tokenizer requires the stated prior tokenizer
    /// </summary>
    [Serializable]
    public class HasSuccessorTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, IHasSuccessorTokenizer<T>
    {
        #region Ctor
        public HasSuccessorTokenizerDecoration(IForwardMovingTokenizer<T> decorated, IForwardMovingTokenizer<T> nextTokenizer)
            : base(decorated)
        {
            Condition.Requires(nextTokenizer).IsNotNull();
            this.NextTokenizer = nextTokenizer;
        }
        #endregion

        #region Fluent Static
        public static HasSuccessorTokenizerDecoration<T> New(IForwardMovingTokenizer<T> decorated, IForwardMovingTokenizer<T> nextTokenizer)
        {
            return new HasSuccessorTokenizerDecoration<T>(decorated, nextTokenizer);
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
        public IForwardMovingTokenizer<T> NextTokenizer { get; private set; }

        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken, out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            var rv = this.Decorated.Parse(source, currentPosition, state, currentToken, out newPosition, out newToken, out newParser);

            newParser = this.NextTokenizer;
            return rv;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            return new HasSuccessorTokenizerDecoration<T>(thing, this.NextTokenizer);
        }
        #endregion
    }

    public static class HasSuccessorTokenizerDecorationExtensions
    {
        public static HasSuccessorTokenizerDecoration<T> HasSuccessor<T>(this IForwardMovingTokenizer<T> decorated, IForwardMovingTokenizer<T> tokenizer)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasSuccessorTokenizerDecoration<T>(decorated, tokenizer);
        }
    }
}


