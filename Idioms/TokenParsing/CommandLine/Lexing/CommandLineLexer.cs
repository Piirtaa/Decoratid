﻿using System;
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
using CuttingEdge.Conditions;

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


        private static int GetPositionOfComplement<T>(this T[] source, T[] left, T[] right)
        {

            //for (int i = 0; i < source.Length; i++)
            //    if (!source[i].Equals(prefix[i]))
            //        return false;

            //validate we start with left
            Condition.Requires(source.StartsWithSegment(left)).IsTrue();

            int unmatchedpairs = 1;
            while (unmatchedpairs > 0)
            {

            }
            for (int i = 0; i < source.Length; i++)
                if (!source[i].Equals(prefix[i]))
                    return false;

        }
        public static IForwardMovingTokenizer<char> BuildLexingLogic(CLConfig config)
        {
            /*
                CommandExamples:

	            Decorating:   @[id1]#HasDateCreated(now)#HasId(id2)#HasName(name1)  
	            Saving:																	.Save()
	            Deleting:	@[id1].Delete()
	            Getting Ness Value:	@[id1]#HasDateCreated.Date
	            Performing Ness Op: @[id1]#HasDateCreated.SetDate(now)#HasBeep.Beep()
	            Conditional Ness: #HasDateCreated.IsAfter(now)
             */
            
            //define common separators/punctuation.  these will terminate tokens
            var at = "@".ToCharArray();
            var dot = ".".ToCharArray();
            var comma = ",".ToCharArray();
            var openParenthesis = "(".ToCharArray();
            var closedParenthesis = ")".ToCharArray();
            var hash = "#".ToCharArray();
            var openBracket = "[".ToCharArray();
            var closedBracket = "]".ToCharArray();

            //define the basic tokens
            var storeTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
                .HasPrefix(at)
                .HasSuffix(false, at, dot, comma, openParenthesis,closedParenthesis, hash, openBracket, closedBracket)
                .HasValueFactory(token =>
                {
                    string storeName = new string(token.TokenData);
                    if (string.IsNullOrEmpty(storeName))
                    {
                        return null;//default store strategy
                    }
                    else
                    {
                        var store = config.StoreOfStores.Get<NamedNaturalInMemoryStore>(storeName);
                        return store;
                    }
                }).HasId("Store"); 

            var idTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
                .HasPrefix(openBracket)
                .HasSuffix(true, closedBracket)
                .HasValueFactory(token =>
                {
                    string id = new string(token.TokenData);
                    return id;
                }).HasId("Id");

            var opTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
                .HasPrefix(dot)
                .HasSuffix(false, dot, openParenthesis)
                .HasId("Op");

            //parse ness name into ness token.  eg. #ness
            var nessTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
                .HasPrefix(hash).HasSuffix(false, dot,openParenthesis)
                .HasValueFactory(token =>
                {
                    string nessName = new string(token.TokenData);

                    //ness is contextual and depends on who is invoking the ness 
                    //so we have to get the prior token's value
                    var lastTokenValueFace = token.PriorToken.GetFace<IHasValue>();
                    var lastTokenValue = lastTokenValueFace.Value;

                    var rv = config.NessManager.GetNess(lastTokenValue, nessName);
                    return rv;
                }).HasId("Ness");

            //parse ness name into ness token.  eg. #ness
            var nessTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
                .HasPrefix(hash).HasSuffix(false, dot, openParenthesis)
                .HasValueFactory(token =>
                {
                    string nessName = new string(token.TokenData);

                    //ness is contextual and depends on who is invoking the ness 
                    //so we have to get the prior token's value
                    var lastTokenValueFace = token.PriorToken.GetFace<IHasValue>();
                    var lastTokenValue = lastTokenValueFace.Value;

                    var rv = config.NessManager.GetNess(lastTokenValue, nessName);
                    return rv;
                }).HasId("Ness");

            router.AddTokenizer(nessTokenizer);



            //to parse this type of syntax we use prefix routing - ie. we route via prefix
            var router = NaturallyNotImplementedForwardMovingTokenizer<char>.New().MakeRouter();
            
            //parse store name into store token.  eg.  @store

            router.AddTokenizer(storeTokenizer);


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
            var rv = text.ToCharArray().ForwardMovingTokenizeToCompletion(null, tokenizer);

            rv.WithEach(x =>
            {

            });

            return rv;
        }
    }
}
