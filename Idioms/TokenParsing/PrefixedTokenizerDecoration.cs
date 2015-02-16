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

namespace Decoratid.Idioms.TokenParsing
{
    /// <summary>
    /// a tokenizer that requires the to-be-parsed token has a valid prefix
    /// </summary>
    public interface IPrefixedTokenizer : IHasHandleConditionTokenizer
    {
        string[] Prefixes { get; }
    }

    /// <summary>
    /// a tokenizer that requires the to-be-parsed token has a valid prefix.  
    /// </summary>
    [Serializable]
    public class PrefixedTokenizerDecoration : ForwardMovingTokenizerDecorationBase, IPrefixedTokenizer
    {
        #region Ctor
        public PrefixedTokenizerDecoration(IForwardMovingTokenizer decorated, params string[] prefixes)
            : base(decorated.HasSelfDirection())
        {
            Condition.Requires(prefixes).IsNotEmpty();
            this.Prefixes = prefixes;
        }
        #endregion

        #region Fluent Static
        public static PrefixedTokenizerDecoration New(IForwardMovingTokenizer decorated, params string[] prefixes)
        {
            return new PrefixedTokenizerDecoration(decorated, prefixes);
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
        public IConditionOf<ForwardMovingTokenizingOperation> CanTokenizeCondition
        {
            get
            {
                //define the prefix condition
                var cond = StrategizedConditionOf<ForwardMovingTokenizingOperation>.New((x) =>
                {
                    var substring = x.Text.Substring(x.CurrentPosition);

                    bool rv = false;

                    foreach (string each in this.Prefixes)
                    {
                        if (substring.StartsWith(each))
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
        public string[] Prefixes { get; private set; }
        private string GetPrefix(string text, int currentPosition)
        {
            var substring = text.Substring(currentPosition);

            foreach (string each in this.Prefixes)
                if (substring.StartsWith(each))
                    return each;

            return string.Empty;
        }

        public override bool Parse(string text, int currentPosition, object state, IToken currentToken, out int newPosition, out IToken newToken, out IForwardMovingTokenizer newParser)
        {
            IToken newTokenOUT = null;

            var rv = base.Parse(text, currentPosition, state, currentToken, out newPosition, out newTokenOUT, out newParser);

            //decorate token with prefix
            var prefix = this.GetPrefix(text, currentPosition);
            newTokenOUT = newTokenOUT.HasPrefix(prefix);

            newToken = newTokenOUT;
            return rv;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer> ApplyThisDecorationTo(IForwardMovingTokenizer thing)
        {
            return new PrefixedTokenizerDecoration(thing, this.Prefixes);
        }
        #endregion
    }

    public static class PrefixedTokenizerDecorationExtensions
    {
        public static PrefixedTokenizerDecoration HasPrefix(this IForwardMovingTokenizer decorated, params string[] prefixes)
        {
            Condition.Requires(decorated).IsNotNull();
            return new PrefixedTokenizerDecoration(decorated, prefixes);
        }
    }
}


