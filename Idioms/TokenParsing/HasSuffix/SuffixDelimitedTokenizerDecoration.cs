﻿using CuttingEdge.Conditions;
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
    public interface ISuffixDelimitedTokenizerDecoration<T> : IForwardMovingTokenizer<T> 
    {
        T[][] Suffixes { get; }

        /// <summary>
        /// does the parser/tokenizer that pulls the raw token data include the suffix, 
        /// or is this determined after the parse?  in other words, does the NEXT cursor position
        /// include the suffix (eg. IsInclusive = false), or does the CURRENT token include the suffix 
        /// (eg. IsInclusive = true).
        /// </summary>
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
            : base(decorated)
        {
            Condition.Requires(suffixes).IsNotEmpty();
            this.Suffixes = suffixes;
            this.IsInclusive = isInclusive;
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


        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken,
            out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            IToken<T> newTokenOUT = null;

            var rv = this.Decorated.Parse(source, currentPosition, state, currentToken, out newPosition, out newTokenOUT, out newParser);


            if (this.IsInclusive)
            {
                //decorate token with suffix
                var suffix = newTokenOUT.TokenData.FindMatchingSuffix(this.Suffixes);
                newTokenOUT = newTokenOUT.HasSuffix(suffix, this.IsInclusive);
            }
            else
            {
                //find which suffix is first
                var suffix = source.FindMatchingPrefix(newPosition, this.Suffixes);
                newTokenOUT = newTokenOUT.HasSuffix(suffix, this.IsInclusive);
            }

            newToken = newTokenOUT;
            return rv;
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


