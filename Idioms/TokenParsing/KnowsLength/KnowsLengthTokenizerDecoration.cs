using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core;
using Decoratid.Extensions;

namespace Decoratid.Idioms.TokenParsing.KnowsLength
{
    /// <summary>
    /// marker interface indicating that the tokenizer has a defined end-point.
    /// Typically your cake should be decorated with this marker (via
    /// another decoration that ctor decorates it) to indicate that the terminal 
    /// parsing point has been found - and thus the thing can be parsed.
    /// </summary>
    /// <remarks>
    /// if your cake doesn't have this decoration somewhere the cake won't work.
    /// if the decoration that adds this layer isn't declared asap, layers underneath it
    /// will have their parsing logic ignored.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public interface IKnowsLengthTokenizerDecoration<T> : ITokenizerDecoration<T>
    {

    }


    [Serializable]
    public class KnowsLengthTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, IKnowsLengthTokenizerDecoration<T>
    {
        #region Ctor
        public KnowsLengthTokenizerDecoration(IForwardMovingTokenizer<T> decorated)
            : base(decorated.KnowsLength())
        {
            //ensure no more than 1 decoration is possible per stack
            if (decorated.HasDecoration<KnowsLengthTokenizerDecoration<T>>())
                throw new InvalidOperationException("already knows length");
        }
        #endregion

        #region Fluent Static
        public static KnowsLengthTokenizerDecoration<T> New(IForwardMovingTokenizer<T> decorated)
        {
            return new KnowsLengthTokenizerDecoration<T>(decorated);
        }
        #endregion

        #region ISerializable
        protected KnowsLengthTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation

        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken, out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            var rv = base.Parse(source, currentPosition, state, currentToken, out newPosition, out newToken, out newParser);
            return rv;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            return new KnowsLengthTokenizerDecoration<T>(thing);
        }
        #endregion
    }

    public static class KnowsLengthTokenizerDecorationExtensions
    {
        /// <summary>
        /// decorates with self direction.  if stack already has this decoration, just ands the condition
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="canHandleStrategy"></param>
        /// <returns></returns>
        public static KnowsLengthTokenizerDecoration<T> KnowsLength<T>(this IForwardMovingTokenizer<T> decorated)
        {
            Condition.Requires(decorated).IsNotNull();

            var dec = decorated.As<KnowsLengthTokenizerDecoration<T>>(true);
            if (dec != null)
            {
                return dec;
            }
            return new KnowsLengthTokenizerDecoration<T>(decorated);
        }

    }
}


