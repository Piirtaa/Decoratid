using CuttingEdge.Conditions;
using Decoratid.Core;
using Decoratid.Idioms.Stringing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core.Conditional.Of;

namespace Decoratid.Idioms.TokenParsing
{

    /*
     *  The parsing approach:
     *      given some text, a position, and a parser - the parser will attempt to parse the text and return 
     *      some return data (the token), the new position in text, and the next parser to continue with.
     * 
     *      It's a chain of responsibility that produces a list of tokens whilst parsing a string.  
     *      The program continues only when there is some text to parse and a parser to parse it with.
     *      Each step of the parse determines whether to continue (ie. to find the next parser).
     *      It's conceptually similar to a Turing machine, with each parsing step being 
     *      equivalent to a step in a computer program.  With Turing (unlike this), he needed 
     *      an infinite amount of text (ie. storage), and he moved back and forth along the text -
     *      enabling loops, and he wrote on the text itself. This approach simply moves forward only, 
     *      and tokenizes the source text.
     */


    /// <summary>
    /// some thing that is parsed out of a string, that has a string value
    /// </summary>
    public interface IToken
    {
        IToken PriorToken { get; set; }
        string TokenString { get; }
    }

    public interface IForwardMovingTokenizer
    {
        /// <summary>
        /// moves forward and parses first token it can.  returns false if can't continue
        /// </summary>
        /// <param name="text"></param>
        /// <param name="currentPosition"></param>
        /// <param name="currentToken"></param>
        /// <param name="newPosition"></param>
        /// <param name="newToken">decorate result token to extend </param>
        /// <param name="newParser"></param>
        /// <returns></returns>
        bool Parse(string text, int currentPosition, object state, IToken currentToken,
            out int newPosition, out IToken newToken, out IForwardMovingTokenizer newParser);
    }

    /// <summary>
    /// encapsulates the state of a tokenizing operation
    /// </summary>
    public class ForwardMovingTokenizingOperation
    {
        public string Text { get; set; }
        public int CurrentPosition { get; set; }
        public object State { get; set; }
        public IToken CurrentToken { get; set; }

        public static ForwardMovingTokenizingOperation New(string text, int currentPosition, object state, IToken currentToken)
        {
            var rv = new ForwardMovingTokenizingOperation();
            rv.Text = text;
            rv.CurrentPosition = currentPosition;
            rv.State = state;
            rv.CurrentToken = currentToken;

            return rv;
        }
    }

    /// <summary>
    /// a tokenizer that has a condition that needs to be passed for the tokenizer to work
    /// </summary>
    public interface IHasHandleConditionTokenizer : IForwardMovingTokenizer
    {
        IConditionOf<ForwardMovingTokenizingOperation> CanTokenizeCondition { get; }
    }

    public static class TokenExtensions
    {
        public static string GetTokenizerId(this IToken token)
        {
            Condition.Requires(token).IsNotNull();

            var tokenizer = token.GetFace<IHasTokenizerId>();
            return tokenizer.With(x => x.TokenizerId);
        }
        public static IStartEndPositionalToken GetStartEnd(this IToken token)
        {
            Condition.Requires(token).IsNotNull();
            var tokenizer = token.GetFace<IStartEndPositionalToken>();
            return tokenizer;
        }
        public static string GetPrefix(this IToken token)
        {
            Condition.Requires(token).IsNotNull();
            var tokenizer = token.GetFace<IHasPrefixToken>();
            return tokenizer.With(x => x.Prefix);
        }
        public static string GetSuffix(this IToken token)
        {
            Condition.Requires(token).IsNotNull();
            var tokenizer = token.GetFace<IHasSuffixToken>();
            return tokenizer.With(x => x.Suffix);
        }
        public static string GetPriorTokenizerId(this IToken token)
        {
            Condition.Requires(token).IsNotNull();
            var tokenizer = token.GetFace<IHasPriorTokenizerIdToken>();
            return tokenizer.With(x => x.PriorTokenizerId);
        }

    }

}
