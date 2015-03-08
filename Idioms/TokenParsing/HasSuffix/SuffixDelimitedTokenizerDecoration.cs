using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Idioms.TokenParsing.HasValidation;
using Decoratid.Core.Conditional.Of;
using Decoratid.Idioms.TokenParsing.KnowsLength;

namespace Decoratid.Idioms.TokenParsing.HasSuffix
{



    /// <summary>
    /// parses from current spot to any of the suffixes
    /// </summary>
    public interface ISuffixDelimitedTokenizerDecoration<T> : IHasHandleConditionTokenizer<T>, IKnowsLengthTokenizerDecoration<T> 
    {
        T[][] Suffixes { get; }
        bool IsInclusive { get;}
    }

    /// <summary>
    /// a tokenizer that tokenizes to the nearest suffix.  Defines the core tokenizing process as a parse and outputs
    /// suffix decorated natural tokens.
    /// </summary>
    [Serializable]
    public class SuffixDelimitedTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, ISuffixDelimitedTokenizerDecoration<T>
    {
        #region Ctor
        public SuffixDelimitedTokenizerDecoration(IForwardMovingTokenizer<T> decorated, bool isInclusive, params T[][] suffixes)
            : base(decorated.KnowsLength())
        {
            Condition.Requires(suffixes).IsNotEmpty();
            this.Suffixes = suffixes;
            this.IsInclusive = isInclusive;

            var cake = this.GetAllDecorations();
        }
        #endregion

        #region Fluent Static
        public static SuffixDelimitedTokenizerDecoration<T> New<T>(IForwardMovingTokenizer<T> decorated, bool isInclusive, params T[][] suffixes)
        {
            return new SuffixDelimitedTokenizerDecoration<T>(decorated, isInclusive, suffixes);
        }
        #endregion

        #region ISerializable
        protected SuffixDelimitedTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public bool IsInclusive { get; private set; }
        public T[][] Suffixes { get; private set; }

        public IConditionOf<ForwardMovingTokenizingCursor<T>> CanTokenizeCondition
        {
            get
            {
                var cond = StrategizedConditionOf<ForwardMovingTokenizingCursor<T>>.New((x) =>
                {
                    T[] seg = null;

                    var closestIdx = x.Source.FindNearestIndexOf(this.Suffixes, out seg, x.CurrentPosition);
                    
                    //if we can't find a suffix, we kack
                    return (closestIdx > -1);
                });
                return cond;
            }
        }

        public override bool Parse(T[] dataToTokenize, int currentPosition, object state, IToken<T> currentToken,
            out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            //for all the suffixes this parses to, finds the nearest one (nongreedy)
            int closestIdx = -1;
            T[] suffix = null;

            closestIdx = dataToTokenize.FindNearestIndexOf(this.Suffixes, out suffix, currentPosition);

            //if we can't find a suffix, we kack
            if (closestIdx == -1)
            {
                newParser = null;
                newToken = null;
                newPosition = -1;
                return false;
            }

            newPosition = closestIdx;

            //get string between old and new positions
            var tokenText = dataToTokenize.GetSegment(currentPosition, newPosition - currentPosition);

            //returns a suffixed natural token
            newToken = NaturalToken<T>.New(tokenText).HasSuffix(suffix, this.IsInclusive);

            //we don't know what parser to use next
            newParser = null;

            return true;
        }

        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            return new SuffixDelimitedTokenizerDecoration<T>(thing,this.IsInclusive, this.Suffixes);
        }
        #endregion
    }

    public static class SuffixDelimitedTokenizerDecorationExtensions
    {
        public static IForwardMovingTokenizer<T> HasSuffix<T>(this IForwardMovingTokenizer<T> decorated, bool isInclusive, params T[][] suffixes)
        {
            Condition.Requires(decorated).IsNotNull();
            return new SuffixDelimitedTokenizerDecoration<T>(decorated, isInclusive, suffixes).HasValidation();
            //NOTE: good practice is to add validation fluently after any decoration that introduces a handling condition
        }
    }
}


