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
    /// generic version of a token.  the common string version would be IToken of char
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IToken<T>
    {
        IToken<T> PriorToken { get; set; }
        T[] TokenData { get; }
    }

    public interface IForwardMovingTokenizer<T>
    {
        bool Parse(T[] source, int currentPosition, object state, IToken<T> currentToken,
            out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser);
    }

    /// <summary>
    /// encapsulates the state of a tokenizing operation
    /// </summary>
    public class ForwardMovingTokenizingOperation<T>
    {
        public T[] Source { get; set; }
        public int CurrentPosition { get; set; }
        public object State { get; set; }
        public IToken<T> CurrentToken { get; set; }

        public static ForwardMovingTokenizingOperation<T> New(T[] source, int currentPosition, object state,
            IToken<T> currentToken)
        {
            var rv = new ForwardMovingTokenizingOperation<T>();
            rv.Source = source;
            rv.CurrentPosition = currentPosition;
            rv.State = state;
            rv.CurrentToken = currentToken;
            return rv;
        }
    }


    
    #region String Implementations

    ///// <summary>
    ///// some thing that is parsed out of a string, that has a string value
    ///// </summary>
    //public interface IStringToken
    //{
    //    IStringToken PriorToken { get; set; }
    //    string TokenString { get; }
    //}

    //public interface IForwardMovingTokenizer<T>
    //{
    //    /// <summary>
    //    /// moves forward and parses first token it can.  returns false if can't continue
    //    /// </summary>
    //    /// <param name="text"></param>
    //    /// <param name="currentPosition"></param>
    //    /// <param name="currentToken"></param>
    //    /// <param name="newPosition"></param>
    //    /// <param name="newToken">decorate result token to extend </param>
    //    /// <param name="newParser"></param>
    //    /// <returns></returns>
    //    bool Parse(string text, int currentPosition, object state, IStringToken currentToken,
    //        out int newPosition, out IStringToken newToken, out IForwardMovingTokenizer<T> newParser);
    //}

    ///// <summary>
    ///// encapsulates the state of a tokenizing operation
    ///// </summary>
    //public class ForwardMovingTokenizingOperation<T>
    //{
    //    public string Text { get; set; }
    //    public int CurrentPosition { get; set; }
    //    public object State { get; set; }
    //    public IStringToken CurrentToken { get; set; }

    //    public static ForwardMovingTokenizingOperation<T> New(string text, int currentPosition, object state, IStringToken currentToken)
    //    {
    //        var rv = new ForwardMovingTokenizingOperation<T>();
    //        rv.Text = text;
    //        rv.CurrentPosition = currentPosition;
    //        rv.State = state;
    //        rv.CurrentToken = currentToken;

    //        return rv;
    //    }
    //}



    public static class TokenExtensions
    {
        
        //public static string GetTokenizerId(this IStringToken token)
        //{
        //    Condition.Requires(token).IsNotNull();

        //    var tokenizer = token.GetFace<IHasTokenizerId>();
        //    return tokenizer.With(x => x.TokenizerId);
        //}
        //public static IStartEndPositionalToken GetStartEnd(this IStringToken token)
        //{
        //    Condition.Requires(token).IsNotNull();
        //    var tokenizer = token.GetFace<IStartEndPositionalToken>();
        //    return tokenizer;
        //}
        //public static string GetPrefix(this IStringToken token)
        //{
        //    Condition.Requires(token).IsNotNull();
        //    var tokenizer = token.GetFace<IHasPrefixToken>();
        //    return tokenizer.With(x => x.Prefix);
        //}
        //public static string GetSuffix(this IStringToken token)
        //{
        //    Condition.Requires(token).IsNotNull();
        //    var tokenizer = token.GetFace<IHasSuffixToken>();
        //    return tokenizer.With(x => x.Suffix);
        //}
        //public static string GetPriorTokenizerId(this IStringToken token)
        //{
        //    Condition.Requires(token).IsNotNull();
        //    var tokenizer = token.GetFace<IHasPriorTokenizerIdToken>();
        //    return tokenizer.With(x => x.PriorTokenizerId);
        //}

    }
    #endregion
}
