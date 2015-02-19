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

namespace Decoratid.Idioms.TokenParsing
{
    /// <summary>
    /// a tokenizer that knows whether it can handle a tokenizing operation
    /// </summary>
    public interface ISelfDirectedTokenizer : IForwardMovingTokenizer
    {
        bool CanHandle(string text, int currentPosition, object state, IToken currentToken);
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
    public class SelfDirectedTokenizerDecoration : ForwardMovingTokenizerDecorationBase, ISelfDirectedTokenizer, IHasHandleConditionTokenizer
    {
        #region Ctor
        public SelfDirectedTokenizerDecoration(IForwardMovingTokenizer decorated, 
            IConditionOf<ForwardMovingTokenizingOperation> canHandleCondition = null)
            : base(decorated)
        {
            //ensure no more than 1 selfdirected decoration is possible per stack
            if (decorated.HasDecoration<SelfDirectedTokenizerDecoration>())
                throw new InvalidOperationException("already self-directed");

            this.CanTokenizeCondition = canHandleCondition;
        }
        #endregion

        #region Fluent Static
        public static SelfDirectedTokenizerDecoration New(IForwardMovingTokenizer decorated, IConditionOf<ForwardMovingTokenizingOperation> canHandleCondition)
        {
            return new SelfDirectedTokenizerDecoration(decorated, canHandleCondition);
        }
        #endregion

        #region ISerializable
        protected SelfDirectedTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public IConditionOf<ForwardMovingTokenizingOperation> CanTokenizeCondition { get; set; }
        public bool CanHandle(string text, int currentPosition, object state, IToken currentToken)
        {
            var cursor = ForwardMovingTokenizingOperation.New(text, currentPosition, state, currentToken);

            //get all IHasHandleConditionTokenizer conditions in the decoration stack
            //-the idea here is that there is only one instance of SelfDirectedTokenizer per decoration stack (it checks during ctor)
            //-this is the only decoration that will filter 
            //-all the other decorations that affect this decoration (eg. all IHasHandleConditionTokenizers) are pulled from
            // the decoration stack and applied here

            var decs = this.GetAllDecorations();

            var hasConditions = this.GetAllImplementingDecorations<IHasHandleConditionTokenizer>();
            List<IConditionOf<ForwardMovingTokenizingOperation>> conds = new List<IConditionOf<ForwardMovingTokenizingOperation>>();
            hasConditions.WithEach(x =>
            {
                if (x.CanTokenizeCondition != null)
                    conds.Add(x.CanTokenizeCondition);
            });

            if (conds.Count == 0)
                return false;

            var cond = AndOf<ForwardMovingTokenizingOperation>.New(conds.ToArray());
            var rv = cond.Evaluate(cursor);
            
            if (!rv.GetValueOrDefault())
                return false;

            return true;
        }
        public override bool Parse(string text, int currentPosition, object state, IToken currentToken, out int newPosition, out IToken newToken, out IForwardMovingTokenizer newParser)
        {
            if (!CanHandle(text, currentPosition, state, currentToken))
                throw new LexingException("cannot tokenize");

            var rv = base.Parse(text, currentPosition, state, currentToken, out newPosition, out newToken, out newParser);
            return rv;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer> ApplyThisDecorationTo(IForwardMovingTokenizer thing)
        {
            return new SelfDirectedTokenizerDecoration(thing, this.CanTokenizeCondition);
        }
        #endregion
    }

    public static class SelfDirectedTokenizerDecorationExtensions
    {
        /// <summary>
        /// decorates with self direction.  if stack already has this decoration, just ands the condition
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="canHandleStrategy"></param>
        /// <returns></returns>
        public static SelfDirectedTokenizerDecoration HasSelfDirection(this IForwardMovingTokenizer decorated, IConditionOf<ForwardMovingTokenizingOperation> canHandleCondition = null)
        {
            Condition.Requires(decorated).IsNotNull();

            //if we have a self direction decoration in the stack we return that
            var dec = decorated.As<SelfDirectedTokenizerDecoration>(true);
            if (dec != null)
            {
                dec.CanTokenizeCondition = dec.CanTokenizeCondition.And(canHandleCondition);
                return dec;
            }
            return new SelfDirectedTokenizerDecoration(decorated, canHandleCondition);
        }

    }
}


