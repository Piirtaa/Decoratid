using CuttingEdge.Conditions;
using Decoratid.Core;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Idioms.TokenParsing.HasId;
using Decoratid.Idioms.TokenParsing.HasValidation;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing.HasRouting
{

    //define behaviour of the logic for each bit
    using TokenizerItem = IsA<IHasId<string>>;


    /// <summary>
    /// this decoration delegates the actual tokenizing process to the appropriate tokenizer. Typically instances of this
    /// are used as the bootstrapping tokenizer
    /// </summary>
    public interface IRoutingTokenizer<T> : IValidatingTokenizer<T>
    {
        IStoreOf<TokenizerItem> Rules { get; }
        IRoutingTokenizer<T> AddTokenizer(IHasStringIdTokenizer<T> t);
        TokenizerItem GetTokenizer(T[] source, int currentPosition, object state, IToken<T> currentToken);
    }

    /// <summary>
    /// this decoration delegates the actual tokenizing process to the appropriate tokenizer
    /// </summary>
    [Serializable]
    public class RoutingTokenizerDecoration<T> : ForwardMovingTokenizerDecorationBase<T>, IRoutingTokenizer<T>
    {
        #region Ctor
        public RoutingTokenizerDecoration(IForwardMovingTokenizer<T> decorated)
            : base(decorated)
        {
            this.Rules = NaturalInMemoryStore.New().IsOf<TokenizerItem>();
        }
        #endregion

        #region Fluent Static
        public static RoutingTokenizerDecoration<T> New(IForwardMovingTokenizer<T> decorated)
        {
            return new RoutingTokenizerDecoration<T>(decorated);
        }
        #endregion

        #region ISerializable
        protected RoutingTokenizerDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Implementation
        public IStoreOf<TokenizerItem> Rules { get; private set; }
        public IRoutingTokenizer<T> AddTokenizer(IHasStringIdTokenizer<T> t)
        {
            //tell each tokenizer to use the router as the backup router
            var newT = t.HasRouting(this, false);

            TokenizerItem item = new TokenizerItem(newT);

            this.Rules.SaveItem(item);
            return this;
        }
        public TokenizerItem GetTokenizer(T[] source, int currentPosition, object state, IToken<T> currentToken)
        {
            //if we're passed the end of the source, return null
            if (source.Length <= currentPosition)
                return null;

            List<TokenizerItem> tokenizers = Rules.GetAll();

            //iterate thru all the tokenizers and find ones that know if they can handle stuff
            foreach (var each in tokenizers)
            {
                var sd = each.GetFace<IValidatingTokenizer<T>>();

                if (sd != null)
                    if (sd.CanHandle(source, currentPosition, state, currentToken))
                        return each;
            }
            return null;
        }

        public virtual bool CanHandle(T[] source, int currentPosition, object state, IToken<T> currentToken)
        {
            var tokenizer = GetTokenizer(source, currentPosition, state, currentToken);
            return tokenizer != null;
        }
        public override bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken, out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            //get the new tokenizer
            var tokenizer = GetTokenizer(source, currentPosition, state, currentToken);

            //skip out with a false
            if (tokenizer == null)
            {
                newParser = null;
                newToken = null;
                newPosition = -1;
                return false;
            }
            IForwardMovingTokenizer<T> alg = tokenizer.As<IForwardMovingTokenizer<T>>();
            var rv = alg.Parse(source, currentPosition, state, currentToken, out newPosition, out newToken, out newParser);
            return rv;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer<T>> ApplyThisDecorationTo(IForwardMovingTokenizer<T> thing)
        {
            var rv = new RoutingTokenizerDecoration<T>(thing);

            //move the rules over
            var rules = this.Rules.GetAll();
            rules.WithEach(x =>
            {
                rv.Rules.SaveItem(x);
            });

            return rv;
        }
        #endregion
    }

    public static class RoutingTokenizerDecorationExtensions
    {
        public static RoutingTokenizerDecoration<T> MakeRouter<T>(this IForwardMovingTokenizer<T> decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new RoutingTokenizerDecoration<T>(decorated);
        }
    }
}


