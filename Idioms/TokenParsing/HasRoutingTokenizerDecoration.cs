using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Storing;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing
{
    /// <summary>
    /// gives a tokenizer routing logic
    /// </summary>
    public interface IHasRoutingTokenizer : IForwardMovingTokenizer
    {
        IRoutingTokenizer Router { get; }

        /// <summary>
        /// if the default behaviour returns the next tokenizer, override it with the routers suggestion
        /// </summary>
        bool OverrideIfNonNull { get; }
    }

    /// <summary>
    /// gives a tokenizer routing logic
    /// </summary>
    [Serializable]
    public class HasRoutingTokenizerDecoration : ForwardMovingTokenizerDecorationBase, IHasRoutingTokenizer
    {
        #region Ctor
        public HasRoutingTokenizerDecoration(IForwardMovingTokenizer decorated, IRoutingTokenizer router, bool overrideIfNonNull)
            : base(decorated)
        {
            Condition.Requires(router).IsNotNull();
            this.Router = router;
            this.OverrideIfNonNull = overrideIfNonNull;
        }
        #endregion

        #region Fluent Static
        public static HasRoutingTokenizerDecoration New(IForwardMovingTokenizer decorated, IRoutingTokenizer router, bool overrideIfNonNull)
        {
            return new HasRoutingTokenizerDecoration(decorated, router, overrideIfNonNull);
        }
        #endregion

        #region ISerializable
        protected HasRoutingTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public IRoutingTokenizer Router { get; private set; }
        /// <summary>
        /// if the default behaviour returns the next tokenizer, override it with the routers suggestion
        /// </summary>
        public bool OverrideIfNonNull { get; private set; }

        public override bool Parse(string text, int currentPosition, object state, IToken currentToken, out int newPosition, out IToken newToken, out IForwardMovingTokenizer newParser)
        {
            var rv = base.Parse(text, currentPosition, state, currentToken, out newPosition, out newToken, out newParser);

            if (newParser == null)
            {
                var tokenizer = this.Router.GetTokenizer(text, newPosition, state, currentToken);
                newParser = tokenizer.As<IForwardMovingTokenizer>();
                return rv;
            }
            else
            {
                //override the new parser to use the router
                if (OverrideIfNonNull)
                {
                    var tokenizer = this.Router.GetTokenizer(text, newPosition, state, currentToken);
                    newParser = tokenizer.As<IForwardMovingTokenizer>();
                    return rv;
                }
            }
            return false;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer> ApplyThisDecorationTo(IForwardMovingTokenizer thing)
        {
            return new HasRoutingTokenizerDecoration(thing, this.Router, this.OverrideIfNonNull);
        }
        #endregion
    }

    public static class HasRoutingTokenizerDecorationExtensions
    {
        public static HasRoutingTokenizerDecoration HasRouting(this IForwardMovingTokenizer decorated, IRoutingTokenizer router, bool overrideIfNonNull)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasRoutingTokenizerDecoration(decorated, router, overrideIfNonNull);
        }
    }
}


