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

namespace Decoratid.Idioms.TokenParsing.HasConstantValue
{
    /// <summary>
    /// a tokenizer that parses constants
    /// </summary>
    public interface IConstantTokenizerDecoration<T> : IHasHandleConditionTokenizer<T>
    {
        T[] TokenValue { get; }
    }

    /// <summary>
    /// a tokenizer that tokenizes to the nearest suffix.  Defines the core tokenizing process as a parse and outputs
    /// suffix decorated natural tokens.
    /// </summary>
    [Serializable]
    public class ConstantTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, IConstantTokenizerDecoration<T>
    {
        #region Ctor
        public ConstantTokenizerDecoration(IForwardMovingTokenizer<T> decorated, T[] tokenValue)
            : base(decorated.HasValidation())
        {
            Condition.Requires(tokenValue).IsNotNull();
            this.TokenValue = tokenValue;
        }
        #endregion

        #region Fluent Static
        public static ConstantTokenizerDecoration<T> New<T>(IForwardMovingTokenizer<T> decorated, T[] tokenValue)
        {
            return new ConstantTokenizerDecoration<T>(decorated, tokenValue);
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
        public T[] TokenValue { get; private set; }
        public IConditionOf<ForwardMovingTokenizingCursor<T>> CanTokenizeCondition
        {
            get
            {
                var cond = StrategizedConditionOf<ForwardMovingTokenizingCursor<T>>.New((x) =>
                {
                    var substring = x.Source.GetSegment(x.CurrentPosition);

                    if (!substring.StartsWithSegment(this.TokenValue))
                        return false;

                    return true;
                });
                return cond;
            }
        }

        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken,
            out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            newPosition = currentPosition + this.TokenValue.Length;

            //returns a natural token
            newToken = NaturalToken<T>.New(this.TokenValue);

            //we don't know what parser to use next
            newParser = null;

            return true;
        }

        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            return new ConstantTokenizerDecoration<T>(thing, this.TokenValue);
        }
        #endregion
    }

    public static class ConstantTokenizerDecorationExtensions
    {
        public static ConstantTokenizerDecoration<T> HasConstantValue<T>(this IForwardMovingTokenizer<T> decorated, T[] tokenValue)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ConstantTokenizerDecoration<T>(decorated, tokenValue);
        }
    }
}


