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

namespace Decoratid.Idioms.TokenParsing.HasRouting
{
    /// <summary>
    /// gives a tokenizer routing logic
    /// </summary>
    public interface IHasRoutingTokenizer<T> : IForwardMovingTokenizer<T>
    {
        IRoutingTokenizer<T> Router { get; }

        /// <summary>
        /// if the default behaviour returns the next tokenizer, override it with the routers suggestion
        /// </summary>
        bool OverrideIfNonNull { get; }
    }

    /// <summary>
    /// gives a tokenizer routing logic
    /// </summary>
    [Serializable]
    public class HasRoutingTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, IHasRoutingTokenizer<T>
    {
        #region Ctor
        public HasRoutingTokenizerDecoration(IForwardMovingTokenizer<T> decorated,
            IRoutingTokenizer<T> router, bool overrideIfNonNull)
            : base(decorated)
        {
            Condition.Requires(router).IsNotNull();
            this.Router = router;
            this.OverrideIfNonNull = overrideIfNonNull;
        }
        #endregion

        #region Fluent Static
        public static HasRoutingTokenizerDecoration<T> New<T>(IForwardMovingTokenizer<T> decorated, IRoutingTokenizer<T> router, bool overrideIfNonNull)
        {
            return new HasRoutingTokenizerDecoration<T>(decorated, router, overrideIfNonNull);
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
        public IRoutingTokenizer<T> Router { get; private set; }
        /// <summary>
        /// if the default behaviour returns the next tokenizer, override it with the routers suggestion
        /// </summary>
        public bool OverrideIfNonNull { get; private set; }

        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken, out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            var rv = base.Parse(source, currentPosition, state, currentToken, out newPosition, out newToken, out newParser);

            if (newParser == null)
            {
                var tokenizer = this.Router.GetTokenizer(source, newPosition, state, newToken);

                if(tokenizer != null)
                    newParser = tokenizer.As<IForwardMovingTokenizer<T>>();
                
                return rv;
            }
            else
            {
                //override the new parser to use the router
                if (OverrideIfNonNull)
                {
                    var tokenizer = this.Router.GetTokenizer(source, newPosition, state, newToken);
                    newParser = tokenizer.As<IForwardMovingTokenizer<T>>();
                    return rv;
                }
            }
            return false;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            return new HasRoutingTokenizerDecoration<T>(thing, this.Router, this.OverrideIfNonNull);
        }
        #endregion
    }

    public static class HasRoutingTokenizerDecorationExtensions
    {
        public static HasRoutingTokenizerDecoration<T> HasRouting<T>(this IForwardMovingTokenizer<T> decorated, IRoutingTokenizer<T> router, bool overrideIfNonNull)
        {
            Condition.Requires(decorated).IsNotNull();
            return new HasRoutingTokenizerDecoration<T>(decorated, router, overrideIfNonNull);
        }
    }
}


