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

namespace Decoratid.Idioms.TokenParsing.HasValidation
{
    /// <summary>
    /// a tokenizer that has a condition that needs to be passed for the tokenizer to work
    /// </summary>
    public interface IHasHandleConditionTokenizer<T> : IForwardMovingTokenizer<T>
    {
        IConditionOf<ForwardMovingTokenizingCursor<T>> CanTokenizeCondition { get; }
    }

    /// <summary>
    /// a tokenizer that knows whether it can handle a tokenizing operation
    /// </summary>
    public interface IValidatingTokenizer<T> : IForwardMovingTokenizer<T>
    {
        bool CanHandle(T[] source, int currentPosition, object state, IToken<T> currentToken);
    }

    /// <summary>
    /// tokenizer that knows if it can handle (ie. tokenize) the text provided.  Only one of this decoration per stack is allowed.
    /// implemented with IHasHandleConditionTokenizer as well to specify implementation of handling filter as IConditionOf 
    /// </summary>
    /// <remarks>
    /// Works in conjunction with IHasHandleConditionTokenizer which is applied on any decoration that has a handling requirement.
    /// Since only one decoration of SelfDirectedTokenizerDecoration is allowed per stack, this is the aggregator point for all
    /// IHasHandleConditionTokenizers on the stack.  All handling conditions on the stack must pass each handler condition.
    /// 
    /// </remarks>
    [Serializable]
    public class ValidatingTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, IValidatingTokenizer<T>, IHasHandleConditionTokenizer<T>
    {
        #region Ctor
        public ValidatingTokenizerDecoration(IForwardMovingTokenizer<T> decorated, 
            IConditionOf<ForwardMovingTokenizingCursor<T>> canHandleCondition = null)
            : base(decorated)
        {
            //ensure no more than 1 selfdirected decoration is possible per stack
            if (decorated.HasDecoration<ValidatingTokenizerDecoration<T>>())
                throw new InvalidOperationException("already self-directed");

            this.CanTokenizeCondition = canHandleCondition;
        }
        #endregion

        #region Fluent Static
        public static ValidatingTokenizerDecoration<T> New(IForwardMovingTokenizer<T> decorated, IConditionOf<ForwardMovingTokenizingCursor<T>> canHandleCondition)
        {
            return new ValidatingTokenizerDecoration<T>(decorated, canHandleCondition);
        }
        #endregion

        #region ISerializable
        protected ValidatingTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public IConditionOf<ForwardMovingTokenizingCursor<T>> CanTokenizeCondition { get; set; }
        public bool CanHandle(T[] source, int currentPosition, object state, IToken<T> currentToken)
        {
            var cursor = ForwardMovingTokenizingCursor<T>.New(source, currentPosition, state, currentToken);

            //get all IHasHandleConditionTokenizer conditions in the decoration stack
            //-the idea here is that there is only one instance of SelfDirectedTokenizer per decoration stack (it checks during ctor)
            //-this is the only decoration that will filter 
            //-all the other decorations that affect this decoration (eg. all IHasHandleConditionTokenizers) are pulled from
            // the decoration stack and applied here

            var decs = this.GetAllDecorations();

            var hasConditions = this.GetAllImplementingDecorations<IHasHandleConditionTokenizer<T>>();
            List<IConditionOf<ForwardMovingTokenizingCursor<T>>> conds = new List<IConditionOf<ForwardMovingTokenizingCursor<T>>>();
            hasConditions.WithEach(x =>
            {
                if (x.CanTokenizeCondition != null)
                    conds.Add(x.CanTokenizeCondition);
            });

            if (conds.Count == 0)
                return false;

            var cond = AndOf<ForwardMovingTokenizingCursor<T>>.New(conds.ToArray());
            var rv = cond.Evaluate(cursor);
            
            if (!rv.GetValueOrDefault())
                return false;

            return true;
        }
        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken, out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            if (!CanHandle(source, currentPosition, state, currentToken))
                throw new LexingException("cannot tokenize");

            var rv = base.Parse(source, currentPosition, state, currentToken, out newPosition, out newToken, out newParser);
            return rv;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            return new ValidatingTokenizerDecoration<T>(thing, this.CanTokenizeCondition);
        }
        #endregion
    }

    public static class ValidatingTokenizerDecorationExtensions
    {
        /// <summary>
        /// decorates with self direction.  if stack already has this decoration, just ands the condition
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="canHandleStrategy"></param>
        /// <returns></returns>
        public static ValidatingTokenizerDecoration<T> HasValidation<T>(this IForwardMovingTokenizer<T> decorated, IConditionOf<ForwardMovingTokenizingCursor<T>> canHandleCondition = null)
        {
            Condition.Requires(decorated).IsNotNull();

            //if we have a self direction decoration in the stack we return that
            var dec = decorated.As<ValidatingTokenizerDecoration<T>>(true);
            if (dec != null)
            {
                dec.CanTokenizeCondition = dec.CanTokenizeCondition.And(canHandleCondition);
                return dec;
            }
            return new ValidatingTokenizerDecoration<T>(decorated, canHandleCondition);
        }

    }
}


