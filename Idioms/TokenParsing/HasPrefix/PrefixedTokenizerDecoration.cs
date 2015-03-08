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

namespace Decoratid.Idioms.TokenParsing.HasPrefix
{
    /// <summary>
    /// a tokenizer that requires the to-be-parsed token has a valid prefix
    /// </summary>
    public interface IPrefixedTokenizer<T> : IHasHandleConditionTokenizer<T>
    {
        T[][] Prefixes { get; }
    }

    /// <summary>
    /// a tokenizer that requires the to-be-parsed token has a valid prefix.  
    /// </summary>
    [Serializable]
    public class PrefixedTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, IPrefixedTokenizer<T>
    {
        #region Ctor
        public PrefixedTokenizerDecoration(IForwardMovingTokenizer<T> decorated, params T[][] prefixes)
            : base(decorated)
        {
            Condition.Requires(prefixes).IsNotEmpty();
            this.Prefixes = prefixes;

            var cake = this.GetAllDecorations();
        }
        #endregion

        #region Fluent Static
        public static PrefixedTokenizerDecoration<T> New(IForwardMovingTokenizer<T> decorated, params T[][] prefixes)
        {
            return new PrefixedTokenizerDecoration<T>(decorated, prefixes);
        }
        #endregion

        #region ISerializable
        protected PrefixedTokenizerDecoration(SerializationInfo info, StreamingContext context)
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
                //define the prefix condition
                var cond = StrategizedConditionOf<ForwardMovingTokenizingCursor<T>>.New((x) =>
                {
                    var substring = x.Source.GetSegment(x.CurrentPosition);

                    bool rv = false;

                    foreach (T[] each in this.Prefixes)
                    {
                        if (substring.StartsWithSegment(each))
                        {
                            rv = true;
                            break;
                        }
                    }

                    if (!rv)
                        return false;

                    return true;
                });
                return cond;
            }
        }
        public T[][] Prefixes { get; private set; }
        private T[] GetPrefix(T[] source, int currentPosition)
        {
            var substring = source.GetSegment(currentPosition);

            foreach (T[] each in this.Prefixes)
                if (substring.StartsWithSegment(each))
                    return each;

            return null;
        }

        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken, out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            IToken<T> newTokenOUT = null;

            var rv = this.Decorated.Parse(source, currentPosition, state, currentToken, out newPosition, out newTokenOUT, out newParser);

            //decorate token with prefix
            var prefix = this.GetPrefix(source, currentPosition);
            newTokenOUT = newTokenOUT.HasPrefix(prefix);

            newToken = newTokenOUT;
            return rv;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            return new PrefixedTokenizerDecoration<T>(thing, this.Prefixes);
        }
        #endregion
    }

    public static class PrefixedTokenizerDecorationExtensions
    {
        public static IForwardMovingTokenizer<T> HasPrefix<T>(this IForwardMovingTokenizer<T> decorated, params T[][] prefixes)
        {
            Condition.Requires(decorated).IsNotNull();
            return new PrefixedTokenizerDecoration<T>(decorated, prefixes).HasValidation();
            //NOTE: good practice is to add validation fluently after any decoration that introduces a handling condition
        }
    }
}


