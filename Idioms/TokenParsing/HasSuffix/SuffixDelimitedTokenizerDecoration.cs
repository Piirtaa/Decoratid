using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Idioms.TokenParsing.HasSuffix
{
    /// <summary>
    /// parses from current spot to any of the suffixes
    /// </summary>
    public interface ISuffixDelimitedTokenizerDecoration<T> : ITokenizerDecoration<T>
    {
        T[][] Suffixes { get; }
    }

    /// <summary>
    /// a tokenizer that tokenizes to the nearest suffix.  Defines the core tokenizing process as a parse and outputs
    /// suffix decorated natural tokens.
    /// </summary>
    [Serializable]
    public class SuffixDelimitedTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, ISuffixDelimitedTokenizerDecoration<T>
    {
        #region Ctor
        public SuffixDelimitedTokenizerDecoration(IForwardMovingTokenizer<T> decorated, params T[][] suffixes)
            : base(decorated)
        {
            Condition.Requires(suffixes).IsNotEmpty();
            this.Suffixes = suffixes;
        }
        #endregion

        #region Fluent Static
        public static SuffixDelimitedTokenizerDecoration<T> New<T>(IForwardMovingTokenizer<T> decorated, params T[][] suffixes)
        {
            return new SuffixDelimitedTokenizerDecoration<T>(decorated, suffixes);
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
        public T[][] Suffixes { get; private set; }

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
            newToken = NaturalToken<T>.New(tokenText).HasSuffix(suffix);

            //we don't know what parser to use next
            newParser = null;

            return true;
        }

        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            return new SuffixDelimitedTokenizerDecoration<T>(thing, this.Suffixes);
        }
        #endregion
    }

    public static class SuffixDelimitedTokenizerDecorationExtensions
    {
        public static SuffixDelimitedTokenizerDecoration<T> HasSuffix<T>(this IForwardMovingTokenizer<T> decorated, params T[][] suffixes)
        {
            Condition.Requires(decorated).IsNotNull();
            return new SuffixDelimitedTokenizerDecoration<T>(decorated, suffixes);
        }
    }
}


