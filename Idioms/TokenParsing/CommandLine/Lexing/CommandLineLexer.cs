using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.TokenParsing.HasConstantValue;
using Decoratid.Idioms.TokenParsing.HasId;
using Decoratid.Idioms.TokenParsing.HasPredecessor;
using Decoratid.Idioms.TokenParsing.HasPrefix;
using Decoratid.Idioms.TokenParsing.HasRouting;
using Decoratid.Idioms.TokenParsing.HasValue;
using Decoratid.Idioms.TokenParsing.HasStartEnd;
using Decoratid.Idioms.TokenParsing.HasSuccessor;
using Decoratid.Idioms.TokenParsing.HasSuffix;
using Decoratid.Idioms.TokenParsing.HasTokenizerId;
using Decoratid.Extensions;
using Decoratid.Core.Storing;
using Decoratid.Core;

namespace Decoratid.Idioms.TokenParsing.CommandLine.Lexing
{
    /// <summary>
    /// Parses a command such as, @store.search(#ness.IsThing("x","y"))
    /// </summary>
    /// <remarks>
    /// uses a hybrid, hand-rolled approach instead of generating from EBNF.
    /// each tokenizer is decorated with some information to describe how it tokenizes.  
    /// tokenizers are brokered using a "router tokenizer" (broker/delegator) that examines the current cursor 
    /// and finds the appropriate tokenizer to use.  some tokenizers are clearly delimited, others are inferred by context 
    /// in the prior token list, and this behaviour is all defined via decoration.  Gives us both complete/explicit token 
    /// classification, and incomplete/implicit token classification (ie.  loose classification). 
    /// </remarks>
    public class CommandLineLexer
    {
        public const string STORE = "store";
        public const string NESS = "ness";
        public const string OP = "op";
        public const string OPENPAREN = "openParen";
        public const string CLOSEPAREN = "closeParen";
        public const string COMMA = "comma";
        public const string ARG = "arg";
        public const string THING = "thing";

        public static IForwardMovingTokenizer<char> BuildLexingLogic(CLConfig config)
        {
            //@store.search(#ness.IsThing("x","y"))
            
            //to parse this type of syntax we use prefix routing - ie. we route via prefix
            var router = NaturallyNotImplementedForwardMovingTokenizer<char>.New().MakeRouter();
            
            //parse store name into store token.  eg.  @store
            var storeTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New().HasPrefix("@".ToCharArray()).HasSuffix(".".ToCharArray())
                .HasValueFactory(token => 
                {
                    string storeName = new string(token.TokenData);
                    var store = config.StoreOfStores.Get<NamedNaturalInMemoryStore>(storeName);
                    return store;
                }).HasId(STORE); 
            router.AddTokenizer(storeTokenizer);

            //parse ness name into ness token.  eg. #ness
            var nessTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New().HasPrefix("#".ToCharArray()).HasSuffix(".".ToCharArray())
                .HasValueFactory(token =>
                {
                    string nessName = new string(token.TokenData);

                    //ness is contextual and depends on who is invoking the ness 
                    //so we have to get the prior token's value
                    var lastTokenValueFace = token.PriorToken.GetFace<IHasValue>();
                    var lastTokenValue = lastTokenValueFace.Value;

                    var rv = config.NessManager.GetNess(lastTokenValue, nessName);
                    return rv;
                }).HasId(NESS);
            router.AddTokenizer(nessTokenizer);

            //parse thing token - isn't a store or a ness or an op since it has no predecessors.  
            //isn't an arg cos it's not terminated by , or ).  eg. "hello" 
            var thingTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New().HasPredecessorTokenizerIds(null).HasSuffix(".".ToCharArray())
                .HasValueFactory(token =>
                {
                    string tokenData = new string(token.TokenData);
                    return tokenData;
                })
                .HasId(THING);
            router.AddTokenizer(thingTokenizer);

            //parse operation name into op token.  eg. .search
            var opTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New().HasPrefix(".".ToCharArray()).HasSuffix(".".ToCharArray(), "(".ToCharArray()).HasId(OP);
            router.AddTokenizer(opTokenizer);

            //open and close brackets.  constants
            var openParenTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New().HasConstantValue("(".ToCharArray()).HasId(OPENPAREN);
            var closeParenTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New().HasConstantValue(")".ToCharArray()).HasId(CLOSEPAREN);
            router.AddTokenizer(openParenTokenizer);
            router.AddTokenizer(closeParenTokenizer);

            //the comma.  constant
            var commaTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New().HasConstantValue(",".ToCharArray()).HasId(COMMA);
            router.AddTokenizer(commaTokenizer);

            //args.  eg. "x", "y"
            //can have no predecessor tokenizer, or ( or ,
            var argTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New().HasPredecessorTokenizerIds(null, OPENPAREN, COMMA).HasSuffix(",".ToCharArray(), ")".ToCharArray())
            .HasValueFactory(token =>
            {
                string tokenData = new string(token.TokenData);
                return tokenData;
            })
            .HasId(ARG);
            router.AddTokenizer(argTokenizer);

            return router;
        }

        public static List<IToken<char>> ForwardMovingTokenize(CLConfig config, string text)
        {
            var tokenizer = BuildLexingLogic(config);
            var rv = text.ToCharArray().ForwardMovingTokenize(null, tokenizer);

            rv.WithEach(x =>
            {

            });

            return rv;
        }
    }
}
