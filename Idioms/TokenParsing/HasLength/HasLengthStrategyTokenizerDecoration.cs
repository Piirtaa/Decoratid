using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core.Logical;
using Decoratid.Idioms.TokenParsing.HasValidation;
using Decoratid.Core.Conditional.Of;

namespace Decoratid.Idioms.TokenParsing.HasLength
{
    public interface IHasLengthStrategyTokenizerDecoration<T> : IHasHandleConditionTokenizer<T>
    {
        LogicOfTo<ForwardMovingTokenizingCursor<T>, int> LengthStrategy { get; }
    }

    /// <summary>
    /// a tokenizer that knows how long the token it will tokenize is.  uses this info to perform the tokenize.
    /// </summary>
    [Serializable]
    public class HasLengthStrategyTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, IHasLengthStrategyTokenizerDecoration<T>
    {
        #region Ctor
        public HasLengthStrategyTokenizerDecoration(IForwardMovingTokenizer<T> decorated,
            LogicOfTo<ForwardMovingTokenizingCursor<T>, int> lengthStrategy)
            : base(decorated)
        {
            Condition.Requires(lengthStrategy).IsNotNull();
            this.LengthStrategy = lengthStrategy;
        }
        #endregion

        #region Fluent Static
        public static HasLengthStrategyTokenizerDecoration<T> New(IForwardMovingTokenizer<T> decorated, LogicOfTo<ForwardMovingTokenizingCursor<T>, int> lengthStrategy)
        {
            return new HasLengthStrategyTokenizerDecoration<T>(decorated, lengthStrategy);
        }
        #endregion

        #region ISerializable
        protected HasLengthStrategyTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// if we have a longer than 0 length we can tokenize
        /// </summary>
        public IConditionOf<ForwardMovingTokenizingCursor<T>> CanTokenizeCondition
        {
            get
            {
                var cond = StrategizedConditionOf<ForwardMovingTokenizingCursor<T>>.New((x) =>
                {
                    var res = this.LengthStrategy.Perform(x) as LogicOfTo<ForwardMovingTokenizingCursor<T>, int>;
                    int length = res.Result;
                    return x.CurrentPosition + length <= x.Source.Length;
                });
                return cond;
            }
        }
        public LogicOfTo<ForwardMovingTokenizingCursor<T>, int> LengthStrategy { get; private set; }

        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken,
            out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            var cursor = ForwardMovingTokenizingCursor<T>.New(source, currentPosition, state, currentToken);
            var logicRes = this.LengthStrategy.Perform(cursor) as LogicOfTo<ForwardMovingTokenizingCursor<T>, int>;
            int length = logicRes.Result;
            newPosition = currentPosition + length;

            //get string between old and new positions
            var tokenText = source.GetSegment(currentPosition, newPosition - currentPosition);

            //returns a suffixed natural token
            newToken = NaturalToken<T>.New(tokenText);

            //we don't know what parser to use next
            newParser = null;

            return true;
        }

        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            return new HasLengthStrategyTokenizerDecoration<T>(thing, this.LengthStrategy);
        }
        #endregion
    }

    public static class HasLengthStrategyTokenizerDecorationExtensions
    {
        public static HasLengthStrategyTokenizerDecoration<T> HasLengthStrategy<T>(this IForwardMovingTokenizer<T> decorated, LogicOfTo<ForwardMovingTokenizingCursor<T>, int> lengthStrategy)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasLengthStrategyTokenizerDecoration<T>(decorated, lengthStrategy);
        }

    }
}


