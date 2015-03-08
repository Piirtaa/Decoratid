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
    /// </summary>
    /// <remarks>
    /// Typically your cake should be decorated with this marker (via
    /// another decoration that ctor decorates it) to indicate that the terminal 
    /// parsing point has been found - and thus the thing can be parsed.
    /// layers underneath the layer that does the actual parse (ie. the layer implementing this)
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
            : base(decorated)
        {
            //ensure no more than 1 decoration is possible per stack

            //checking for this is a bit subtle.   at this point in the decoration process
            // SetDecorated has been called by the base ctor, which puts ValidatingTokenizerDecoration
            // as the topmost decoration.  So any AS call will return this, as it walks to topmost first.
            //We need to check beneath this layer rather, and do an ASBelow call or we will always kack
            var layer = decorated.AsBelow<KnowsLengthTokenizerDecoration<T>>();

            if (layer != null)
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
            var rv = this.Decorated.Parse(source, currentPosition, state, currentToken, out newPosition, out newToken, out newParser);
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

            return new KnowsLengthTokenizerDecoration<T>(decorated);
        }

    }
}


