using Decoratid.Core.Decorating;
using Decoratid.Extensions;
using Decoratid.Idioms.TokenParsing.HasComment;
using Decoratid.Idioms.TokenParsing.HasComposite;
using Decoratid.Idioms.TokenParsing.HasStartEnd;
using Decoratid.Idioms.TokenParsing.HasTokenizerId;
using System;
using System.Collections.Generic;
using System.Text;


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
    public class ForwardMovingTokenizingCursor<T>
    {
        public T[] Source { get; set; }
        public int CurrentPosition { get; set; }
        public object State { get; set; }
        public IToken<T> CurrentToken { get; set; }

        public static ForwardMovingTokenizingCursor<T> New(T[] source, int currentPosition, object state,
            IToken<T> currentToken)
        {
            var rv = new ForwardMovingTokenizingCursor<T>();
            rv.Source = source;
            rv.CurrentPosition = currentPosition;
            rv.State = state;
            rv.CurrentToken = currentToken;
            return rv;
        }
    }

    public static class TokenizingExtensions
    {
        public static string DumpToken<T>(this IToken<T> token)
        {
            List<Tuple<int, string>> state = new List<Tuple<int, string>>();
            dumpToken(token, 0, state);

            return DumpLeveledLines(state);
        }
        private static void dumpToken<T>(IToken<T> token, int indentLevel, List<Tuple<int,string>> state)
        {
            var rv = string.Format("TokenizerId:{0} StartPos:{1} Data:{2} Comment:{3}",
                token.As<IHasTokenizerId<T>>(false).With(x => x.TokenizerId),
                token.As<IStartEndPositionalToken<T>>(false).WithValueType(x => x.StartPos),
                string.Join("", token.TokenData),
                token.As<IHasCommentToken<T>>(false).With(x => x.Comment));

            state.Add(new Tuple<int, string>(indentLevel, rv));

            var comp = token.As<HasCompositeTokenDecoration<T>>();
            if (comp != null && comp.ChildTokens != null)
            {
                foreach (var each in comp.ChildTokens)
                    dumpToken(each, indentLevel + 1, state);
            }
        }

        private static string DumpLeveledLines(List<Tuple<int, string>> data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var each in data)
            {
                if(each.Item2.EndsWith(Environment.NewLine))
                    sb.Append("/t".RepeatString(each.Item1) + each.Item2);
                else
                    sb.AppendLine("/t".RepeatString(each.Item1) + each.Item2);
                
            }

            return sb.ToString();
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



    //public static class TokenExtensions
    //{
        





    //}
    #endregion
}
