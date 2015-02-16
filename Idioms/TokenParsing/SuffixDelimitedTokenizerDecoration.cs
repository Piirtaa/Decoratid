using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing
{
    /// <summary>
    /// parses from current spot to any of the suffixes
    /// </summary>
    public interface ISuffixDelimitedTokenizerDecoration : ITokenizerDecoration
    {
        string[] Suffixes { get; }
    }

    /// <summary>
    /// a tokenizer that tokenizes to the nearest suffix.  Defines the core tokenizing process as a parse and outputs
    /// suffix decorated natural tokens.
    /// </summary>
    [Serializable]
    public class SuffixDelimitedTokenizerDecoration : ForwardMovingTokenizerDecorationBase, ISuffixDelimitedTokenizerDecoration
    {
        #region Ctor
        public SuffixDelimitedTokenizerDecoration(IForwardMovingTokenizer decorated, params string[] suffixes)
            : base(decorated)
        {
            Condition.Requires(suffixes).IsNotEmpty();
            this.Suffixes = suffixes;
        }
        #endregion

        #region Fluent Static
        public static SuffixDelimitedTokenizerDecoration New(IForwardMovingTokenizer decorated, params string[] suffixes)
        {
            return new SuffixDelimitedTokenizerDecoration(decorated, suffixes);
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
        public string[] Suffixes { get; private set; }

        public override bool Parse(string text, int currentPosition, object state, IToken currentToken,
            out int newPosition, out IToken newToken, out IForwardMovingTokenizer newParser)
        {
            //for all the suffixes this parses to, finds the nearest one (nongreedy)
            int closestIdx = -1;
            string suffix = null;

            foreach (var each in Suffixes)
            {
                var tempIdx = text.Substring(currentPosition + 1).IndexOf(each);

                //can't find the suffix, move along
                if (tempIdx == -1)
                    continue;

                //set the closest index if it's undefined
                if (closestIdx == -1)
                {
                    closestIdx = tempIdx;
                    suffix = each;
                    continue;
                }
                //update the closest index
                if (tempIdx < closestIdx)
                {
                    closestIdx = tempIdx;
                    suffix = each;
                    continue;
                }
            }

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
            var tokenText = text.Substring(currentPosition, newPosition - currentPosition);

            //returns a suffixed natural token
            newToken = NaturalToken.New(tokenText).HasSuffix(suffix);

            //we don't know what parser to use next
            newParser = null;

            return true;
        }

        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer> ApplyThisDecorationTo(IForwardMovingTokenizer thing)
        {
            return new SuffixDelimitedTokenizerDecoration(thing, this.Suffixes);
        }
        #endregion
    }

    public static class SuffixDelimitedTokenizerDecorationExtensions
    {
        public static SuffixDelimitedTokenizerDecoration HasSuffix(this IForwardMovingTokenizer decorated, params string[] suffixes)
        {
            Condition.Requires(decorated).IsNotNull();
            return new SuffixDelimitedTokenizerDecoration(decorated, suffixes);
        }
    }
}


