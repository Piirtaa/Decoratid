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
using Decoratid.Idioms.TokenParsing.KnowsLength;

namespace Decoratid.Idioms.TokenParsing.HasLength
{
    /// <summary>
    /// the tokenizer knows how to calculate the length of its parse.  
    /// -if the calculated length exceeds the length available, we cannot handle -> thus IHasHandleCondition
    /// -if the calculated length is 0, we cannot handle -> thus IHasHandleCondition
    /// -marked with IKnowsLength to declare that this layer defines the parse length
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasLengthStrategyTokenizerDecoration<T> : IHasHandleConditionTokenizer<T>, IKnowsLengthTokenizerDecoration<T>
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
            : base(decorated.KnowsLength())
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
        public IConditionOf<ForwardMovingTokenizingCursor<T>> CanTokenizeCondition
        {
            get
            {
                var cond = StrategizedConditionOf<ForwardMovingTokenizingCursor<T>>.New((x) =>
                {
                    //calculate the length
                    var res = this.LengthStrategy.Perform(x) as LogicOfTo<ForwardMovingTokenizingCursor<T>, int>;
                    int length = res.Result;

                    if (x.CurrentPosition + length > x.Source.Length)
                        return false;

                    if (length <= 0)
                        return false;

                    return true;
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
        public static IForwardMovingTokenizer<T> HasLengthStrategy<T>(this IForwardMovingTokenizer<T> decorated,
            LogicOfTo<ForwardMovingTokenizingCursor<T>, int> lengthStrategy)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasLengthStrategyTokenizerDecoration<T>(decorated, lengthStrategy).HasValidation();
            //NOTE: good practice is to add validation fluently after any decoration that introduces a handling condition
        }
        public static IForwardMovingTokenizer<T> HasSuffixDelimitedLengthStrategy<T>(this IForwardMovingTokenizer<T> decorated,
                params T[][] suffixes)
        {
            Condition.Requires(decorated).IsNotNull();

            var lengthStrategy = LogicOfTo<ForwardMovingTokenizingCursor<T>, int>.New(x =>
            {
                int closestIdx = -1;
                T[] suffix = null;

                closestIdx = x.Source.FindNearestIndexOf(suffixes, out suffix, x.CurrentPosition);

                if (closestIdx <= 0)
                    return 0;

                var rv = closestIdx - x.CurrentPosition;

                return rv;
            });

            return new HasLengthStrategyTokenizerDecoration<T>(decorated, lengthStrategy).HasValidation();
            //NOTE: good practice is to add validation fluently after any decoration that introduces a handling condition
        }
        public static IForwardMovingTokenizer<T> HasPairDelimitedLengthStrategy<T>(this IForwardMovingTokenizer<T> decorated,
        T[] prefix, T[] suffix)
        {
            Condition.Requires(decorated).IsNotNull();
            Condition.Requires(prefix).IsNotNull().IsNotEmpty();
            Condition.Requires(suffix).IsNotNull().IsNotEmpty();

            var lengthStrategy = LogicOfTo<ForwardMovingTokenizingCursor<T>, int>.New(x =>
            {
                var pos = x.Source.GetPositionOfComplement(prefix, suffix, x.CurrentPosition);
                var rv = pos - x.CurrentPosition;

                if (rv <= 0)
                    return 0;
                return rv;
            });

            return new HasLengthStrategyTokenizerDecoration<T>(decorated, lengthStrategy).HasValidation();
            //NOTE: good practice is to add validation fluently after any decoration that introduces a handling condition
        }


    }
}


