using CuttingEdge.Conditions;
using Decoratid.Core.Conditional.Of;
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
    /// a tokenizer that parses string constants
    /// </summary>
    public interface IConstantTokenizerDecoration : IHasHandleConditionTokenizer
    {
        string TokenValue { get; }
    }

    /// <summary>
    /// a tokenizer that tokenizes to the nearest suffix.  Defines the core tokenizing process as a parse and outputs
    /// suffix decorated natural tokens.
    /// </summary>
    [Serializable]
    public class ConstantTokenizerDecoration : ForwardMovingTokenizerDecorationBase, IConstantTokenizerDecoration
    {
        #region Ctor
        public ConstantTokenizerDecoration(IForwardMovingTokenizer decorated, string tokenValue)
            : base(decorated.HasSelfDirection())
        {
            Condition.Requires(tokenValue).IsNotNullOrEmpty();
            this.TokenValue = tokenValue;
        }
        #endregion

        #region Fluent Static
        public static ConstantTokenizerDecoration New(IForwardMovingTokenizer decorated, string tokenValue)
        {
            return new ConstantTokenizerDecoration(decorated, tokenValue);
        }
        #endregion

        #region ISerializable
        protected ConstantTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public string TokenValue { get; private set; }
        public IConditionOf<ForwardMovingTokenizingOperation> CanTokenizeCondition
        {
            get
            {
                var cond = StrategizedConditionOf<ForwardMovingTokenizingOperation>.New((x) =>
                {
                    var substring = x.Text.Substring(x.CurrentPosition);

                    if (!substring.StartsWith(this.TokenValue))
                        return false;

                    return true;
                });
                return cond;
            }
        }

        public override bool Parse(string text, int currentPosition, object state, IToken currentToken,
            out int newPosition, out IToken newToken, out IForwardMovingTokenizer newParser)
        {
            newPosition = currentPosition + this.TokenValue.Length;

            //returns a natural token
            newToken = NaturalToken.New(this.TokenValue);

            //we don't know what parser to use next
            newParser = null;

            return true;
        }

        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer> ApplyThisDecorationTo(IForwardMovingTokenizer thing)
        {
            return new ConstantTokenizerDecoration(thing, this.TokenValue);
        }
        #endregion
    }

    public static class ConstantTokenizerDecorationExtensions
    {
        public static ConstantTokenizerDecoration HasConstantValue(this IForwardMovingTokenizer decorated, string tokenValue)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ConstantTokenizerDecoration(decorated, tokenValue);
        }
    }
}


