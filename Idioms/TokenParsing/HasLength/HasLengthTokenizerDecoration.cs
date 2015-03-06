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

namespace Decoratid.Idioms.TokenParsing.HasLength
{
    public interface IHasLengthTokenizerDecoration<T> : IHasHandleConditionTokenizer<T>, IKnowsLengthTokenizerDecoration<T> 
    {
        int Length { get; }
    }

    /// <summary>
    /// a tokenizer that tokenizes to the nearest suffix.  Defines the core tokenizing process as a parse and outputs
    /// suffix decorated natural tokens.
    /// </summary>
    [Serializable]
    public class HasLengthTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, IHasLengthTokenizerDecoration<T>
    {
        #region Ctor
        public HasLengthTokenizerDecoration(IForwardMovingTokenizer<T> decorated, int length)
            : base(decorated.KnowsLength())
        {
            Condition.Requires(length).IsGreaterThan(0);
            this.Length = length;
        }
        #endregion

        #region Fluent Static
        public static HasLengthTokenizerDecoration<T> New<T>(IForwardMovingTokenizer<T> decorated, int length)
        {
            return new HasLengthTokenizerDecoration<T>(decorated, length);
        }
        #endregion

        #region ISerializable
        protected HasLengthTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public int Length { get; private set; }

        public IConditionOf<ForwardMovingTokenizingCursor<T>> CanTokenizeCondition
        {
            get
            {
                var cond = StrategizedConditionOf<ForwardMovingTokenizingCursor<T>>.New((x) =>
                {
                    return x.CurrentPosition + this.Length <= x.Source.Length;    
                });
                return cond;
            }
        }

        public override bool Parse(T[] dataToTokenize, int currentPosition, object state, IToken<T> currentToken,
            out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            newPosition = currentPosition + Length;

            //get string between old and new positions
            var tokenText = dataToTokenize.GetSegment(currentPosition, newPosition - currentPosition);

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
            return new HasLengthTokenizerDecoration<T>(thing, this.Length);
        }
        #endregion
    }

    public static class HasLengthTokenizerDecorationExtensions
    {
        public static HasLengthTokenizerDecoration<T> HasLength<T>(this IForwardMovingTokenizer<T> decorated, int length)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasLengthTokenizerDecoration<T>(decorated, length);
        }
    }
}


