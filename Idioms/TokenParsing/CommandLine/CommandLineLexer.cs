using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing.CommandLine
{
    /// <summary>
    /// Parses a command such as, @store.search(#ness.IsThing("x","y"))
    /// </summary>
    /// <remarks>
    /// "@" - parses store name to "."
    /// "." - parses operation name to "(" or "."
    /// "(" - parses nothing (eg. from "(" to itself) , just a placeholder indicating params
    /// "," - parses nothing, just a placeholder for params separator
    /// ")" - parses nothing, just a placeholder indicating params have closed
    /// "#" - parses ness name to "."
    /// everything else parses to literal
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

        public static IForwardMovingTokenizer BuildLexingLogic()
        {
            //@store.search(#ness.IsThing("x","y"))

            //to parse this type of syntax we use prefix routing - ie. we route via prefix
            var router = NaturallyNotImplementedForwardMovingTokenizer.New().MakeRouter();
            
            //parse store name into store token
            var storeTokenizer = NaturallyNotImplementedForwardMovingTokenizer.New().HasPrefix("@").HasSuffix(".").HasId(STORE);
            router.AddTokenizer(storeTokenizer);

            //parse ness name into ness token
            var nessTokenizer = NaturallyNotImplementedForwardMovingTokenizer.New().HasPrefix("#").HasSuffix(".").HasId(NESS);
            router.AddTokenizer(nessTokenizer);

            //parse operation name into op token
            var opTokenizer = NaturallyNotImplementedForwardMovingTokenizer.New().HasPrefix(".").HasSuffix(".", "(").HasId(OP);
            router.AddTokenizer(opTokenizer);

            //open and close brackets
            var openParenTokenizer = NaturallyNotImplementedForwardMovingTokenizer.New().HasConstantValue("(").HasId(OPENPAREN);
            var closeParenTokenizer = NaturallyNotImplementedForwardMovingTokenizer.New().HasConstantValue(")").HasId(CLOSEPAREN);
            router.AddTokenizer(openParenTokenizer);
            router.AddTokenizer(closeParenTokenizer);

            //the comma
            var commaTokenizer = NaturallyNotImplementedForwardMovingTokenizer.New().HasConstantValue(",").HasId(COMMA);
            router.AddTokenizer(commaTokenizer);

            //everything else
            var argTokenizer = NaturallyNotImplementedForwardMovingTokenizer.New().HasPredecessor(OPENPAREN, COMMA).HasSuffix(",", ")").HasId(ARG);
            router.AddTokenizer(argTokenizer);

            return router;
        }

        public static List<IToken> ForwardMovingTokenize(string text)
        {
            var tokenizer = BuildLexingLogic();
            var rv = text.ForwardMovingTokenize(null, tokenizer);
            return rv;
        }
    }
}
