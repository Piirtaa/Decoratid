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
using Decoratid.Core.Identifying;
using Decoratid.Core;
using Decoratid.Extensions;

namespace Decoratid.Idioms.TokenParsing
{

    //define behaviour of the logic for each bit
    using TokenizerItem = IsA<IHasId<string>, IForwardMovingTokenizer>;
    using Decoratid.Core.Conditional.Of;

    /// <summary>
    /// this decoration delegates the actual tokenizing process to the appropriate tokenizer. Typically instances of this
    /// are used as the bootstrapping tokenizer
    /// </summary>
    public interface IRoutingTokenizer : ISelfDirectedTokenizer
    {
        IStoreOf<TokenizerItem> Rules { get; }
        TokenizerItem AddTokenizer(IHasStringIdTokenizer t);
        TokenizerItem GetTokenizer(string text, int currentPosition, object state, IToken currentToken);
    }

    /// <summary>
    /// this decoration delegates the actual tokenizing process to the appropriate tokenizer
    /// </summary>
    [Serializable]
    public class RoutingTokenizerDecoration : ForwardMovingTokenizerDecorationBase, IRoutingTokenizer
    {
        #region Ctor
        public RoutingTokenizerDecoration(IForwardMovingTokenizer decorated)
            : base(decorated)
        {
            this.Rules = NaturalInMemoryStore.New().IsOf<TokenizerItem>();
        }
        #endregion

        #region Fluent Static
        public static RoutingTokenizerDecoration New(IForwardMovingTokenizer decorated)
        {
            return new RoutingTokenizerDecoration(decorated);
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
        public TokenizerItem AddTokenizer(IHasStringIdTokenizer t)
        {
            //tell each tokenizer to use the router as the backup router
            var newT = t.HasRouting(this, false);


            TokenizerItem item = new TokenizerItem(newT);

            this.Rules.SaveItem(item);
            return item;
        }
        public TokenizerItem GetTokenizer(string text, int currentPosition, object state, IToken currentToken)
        {
            List<TokenizerItem> tokenizers = Rules.GetAll();

            //iterate thru all the tokenizers and find ones that know if they can handle stuff
            foreach (var each in tokenizers)
            {
                //get the IForwardMovingTokenizer face
                var tokenizer = each.As<IForwardMovingTokenizer>();
                var tokedecs = tokenizer.GetAllDecorations();
                var tokenizer2 = each.As<ISelfDirectedTokenizer>();
                var tokedecs2 = tokenizer2.GetAllDecorations();
                if (each.Is<ISelfDirectedTokenizer>())
                {
                    if (each.As<ISelfDirectedTokenizer>().CanHandle(text, currentPosition, state,currentToken))
                        return each;
                }
            }
            return null;
        }

        public bool CanHandle(string text, int currentPosition, object state, IToken currentToken)
        {
            var tokenizer = GetTokenizer(text, currentPosition, state, currentToken);
            return tokenizer != null;
        }
        public override bool Parse(string text, int currentPosition, object state, IToken currentToken, out int newPosition, out IToken newToken, out IForwardMovingTokenizer newParser)
        {
            //get the new tokenizer
            var tokenizer = GetTokenizer(text, currentPosition, state, currentToken);

            //skip out with a false
            if (tokenizer == null)
            {
                newParser = null;
                newToken = null;
                newPosition = -1;
                return false;
            }
            IForwardMovingTokenizer alg = tokenizer.As<IForwardMovingTokenizer>();
            var rv = alg.Parse(text, currentPosition, state, currentToken, out newPosition, out newToken, out newParser);
            return rv;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IForwardMovingTokenizer> ApplyThisDecorationTo(IForwardMovingTokenizer thing)
        {
            var rv =  new RoutingTokenizerDecoration(thing);

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
        public static RoutingTokenizerDecoration MakeRouter(this IForwardMovingTokenizer decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new RoutingTokenizerDecoration(decorated);
        }
    }
}


