﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.TokenParsing.HasComposite;
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
using Decoratid.Idioms.TokenParsing.HasLength;
using Decoratid.Extensions;
using Decoratid.Core.Storing;
using Decoratid.Core;
using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using Decoratid.Core.Decorating;

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
            /*
                CommandExamples:

	            Decorating:   @[id1]#HasDateCreated(now)#HasId(id2)#HasName(name1)  
	            Saving:																	.Save()
	            Deleting:	@[id1].Delete()
	            Getting Ness Value:	@[id1]#HasDateCreated.Date
	            Performing Ness Op: @[id1]#HasDateCreated.SetDate(now)#HasBeep.Beep()
	            Conditional Ness: #HasDateCreated.IsAfter(now)
             * 
             * 
             */

            //define common separators/punctuation.  these will terminate tokens
            var at = "@".ToCharArray();
            var dot = ".".ToCharArray();
            var comma = ",".ToCharArray();
            var openParenthesis = "(".ToCharArray();
            var closeParenthesis = ")".ToCharArray();
            var hash = "#".ToCharArray();
            var openBracket = "[".ToCharArray();
            var closeBracket = "]".ToCharArray();
            var emptyParenthesis = "()".ToCharArray();

            #region Primitive-y Tokenizers - all routing unhydrated
            //store token starts with @ and ends with any punctuation
            var storeTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
                .HasSuffix(false, at, dot, comma, openParenthesis, closeParenthesis, hash, openBracket, closeBracket)
                .HasPrefix(at)
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
                })
                .HasId("Store");

            //id token starts with [ and ends with ]. brackets are non-nesting
            var idTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
                .HasSuffix(true, closeBracket)
                .HasPrefix(openBracket)
                .HasValueFactory(token =>
                {
                    string id = new string(token.TokenData);
                    return id;
                }).HasId("Id");

            //op token starts with . and ends with ( or .
            var opTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
                .HasSuffix(false, dot, openParenthesis)
                .HasPrefix(dot)
                .HasId("Op");

            //ness token starts with # and ends with ( or .
            var nessTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
                .HasSuffix(false, dot, openParenthesis)
                .HasPrefix(hash)
                .HasValueFactory(token =>
                {
                    string nessName = new string(token.TokenData);

                    //ness is contextual and depends on who is invoking the ness 
                    //so we have to get the prior token's value
                    var lastTokenValueFace = token.PriorToken.GetFace<IHasValue>();
                    var lastTokenValue = lastTokenValueFace.Value;

                    var rv = config.NessManager.GetNess(lastTokenValue, nessName);
                    return rv;
                })
                .HasId("Ness");

            var commaTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
                .HasConstantValue(comma)
                .HasId("Comma");

            var emptyParenthesisTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
                .HasConstantValue(emptyParenthesis)
                .HasId("EmptyParenthesis");
            #endregion

            //now build the compound tokenizers
            var mainRouterStack = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
                .MakeRouter(true, true)
                .HasId("MainRouter");
            var mainRouter =  mainRouterStack.As<IRoutingTokenizer<char>>(false);
            
            //parenthesis token starts with ( and ends with ), and handles nesting   
            //is a compound that recurses the whole stack 
            //uses the mainRouter to recurse, but is not registered with it as it's not a high level token
            var parenthesisTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
                .HasLengthStrategy(LogicOfTo<ForwardMovingTokenizingCursor<char>, int>.New(cursor =>
                {
                    var rv = GetPositionOfComplement(cursor.Source, openParenthesis, closeParenthesis, cursor.CurrentPosition);

                    return rv - cursor.CurrentPosition;
                }))
                .HasPrefix(openParenthesis)
                .MakeComposite(mainRouter)
                .HasId("Parenthesis");

            //Decorating:   @[id1]#HasDateCreated(now)#HasId(id2)#HasName(name1)  
            //var decoratingCmdTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
            //    .MakeCompositeOf(storeTokenizer, idTokenizer, opTokenizer, nessTokenizer, parenthesisTokenizer, commaTokenizer)
            //    .HasId("Decorating");

            //object manipulation
            //Saving:@[id1].Save()
            //Deleting:	@[id1].Delete()
            //Getting Ness Value:	@[id1]#HasDateCreated.Date
            //Performing Ness Op: @[id1]#HasDateCreated.SetDate(now)#HasBeep.Beep()
            //Conditional Ness: #HasDateCreated.IsAfter(now)
            //var hasIdCmdTokenizer = NaturallyNotImplementedForwardMovingTokenizer<char>.New()
            //    .MakeCompositeOf(storeTokenizer, idTokenizer, nessTokenizer, parenthesisTokenizer)
            //    .HasId("HasIdCommand");

           // mainRouter.AddTokenizer(hasIdCmdTokenizer);//.AddTokenizer(decoratingCmdTokenizer).
            mainRouter.AddTokenizer(storeTokenizer, idTokenizer, opTokenizer, nessTokenizer, commaTokenizer, parenthesisTokenizer);
            return mainRouterStack;
        }

        public static List<IToken<char>> ForwardMovingTokenize(CLConfig config, string text)
        {
            var tokenizer = BuildLexingLogic(config);
            int newPos;
            var rv = text.ToCharArray().ForwardMovingTokenize(null, tokenizer, out newPos);

            rv.WithEach(x =>
            {

            });

            return rv;
        }
        /// <summary>
        /// Parsing helper.  Looks for the next "right" segment such that the count of "left" and "right" segments
        /// are equal.  This ensures we get a well-formed bracketing of left and right.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public static int GetPositionOfComplement<T>( T[] source, T[] left, T[] right, int startPos = 0)
        {
            //  Condition.Requires(source.StartsWithSegment(left)).IsTrue();

            int pos = startPos;
            int unmatchedpairs = 0;
            for (int i = startPos; i < source.Length; i++)
            {
                pos = i;

                if (source.StartsWithSegment(left, i))
                {
                    unmatchedpairs++;
                    i += left.Length;
                }
                if (source.StartsWithSegment(right, i))
                {
                    unmatchedpairs--;
                    i += left.Length;
                }
                if (unmatchedpairs == 0)
                    break;
            }
            return pos;
        }
    }
}
