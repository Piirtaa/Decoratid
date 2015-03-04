using CuttingEdge.Conditions;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Idioms.TokenParsing.HasValidation;

namespace Decoratid.Idioms.TokenParsing.HasImplementation
{

    public interface IHasImplementationTokenizer<T> : IHasHandleConditionTokenizer<T>
    {
        Func<ForwardMovingTokenizingCursor<T>, ForwardMovingTokenizingCursor<T>> TokenizingStrategy { get; }
    }


    [Serializable]
    public class HasImplementationTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, IHasImplementationTokenizer<T>
    {
        #region Ctor
        public HasImplementationTokenizerDecoration(IForwardMovingTokenizer<T> decorated, 
            Func<ForwardMovingTokenizingCursor<T>, ForwardMovingTokenizingCursor<T>> tokenizingStrategy,
            IConditionOf<ForwardMovingTokenizingCursor<T>> canHandleCondition = null)
            : base(decorated.HasValidation(canHandleCondition))
        {
            Condition.Requires(tokenizingStrategy).IsNotNull();
            this.TokenizingStrategy = tokenizingStrategy;
        }
        #endregion

        #region Fluent Static
        public static HasImplementationTokenizerDecoration<T> New(IForwardMovingTokenizer<T> decorated,
            Func<ForwardMovingTokenizingCursor<T>, ForwardMovingTokenizingCursor<T>> tokenizingStrategy,
            IConditionOf<ForwardMovingTokenizingCursor<T>> canHandleCondition = null)
        {
            return new HasImplementationTokenizerDecoration<T>(decorated, tokenizingStrategy, canHandleCondition);
        }
        #endregion

        #region ISerializable
        protected HasImplementationTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public Func<ForwardMovingTokenizingCursor<T>, ForwardMovingTokenizingCursor<T>> TokenizingStrategy { get; private set; }

        public IConditionOf<ForwardMovingTokenizingCursor<T>> CanTokenizeCondition
        {
            get
            {
                return this.As<ValidatingTokenizerDecoration<T>>(true).CanTokenizeCondition;
            }
        }

        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken, 
            out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            var rv = this.TokenizingStrategy(ForwardMovingTokenizingCursor<T>.New(source, currentPosition, state, currentToken));
            newPosition = rv.CurrentPosition;
            newToken = rv.CurrentToken;
            newParser = null;
            return true;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            return new HasImplementationTokenizerDecoration<T>(thing, this.TokenizingStrategy, this.CanTokenizeCondition);
        }
        #endregion
    }

    public static class HasImplementationTokenizerDecorationExtensions
    {
        public static HasImplementationTokenizerDecoration<T> HasImplementation<T>(this IForwardMovingTokenizer<T> decorated,
            Func<ForwardMovingTokenizingCursor<T>, ForwardMovingTokenizingCursor<T>> tokenizingStrategy,
            IConditionOf<ForwardMovingTokenizingCursor<T>> canHandleCondition = null)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasImplementationTokenizerDecoration<T>(decorated, tokenizingStrategy, canHandleCondition);
        }
    }
}


