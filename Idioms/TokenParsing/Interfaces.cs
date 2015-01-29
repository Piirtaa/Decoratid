﻿using CuttingEdge.Conditions;
using Decoratid.Idioms.Stringing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public interface IHasToken
    {
        IToken Token { get; }
    }
    /// <summary>
    /// some thing that is parsed out of a string, that has a string value
    /// </summary>
    public interface IToken
    {
        IToken PriorToken { get; set; }
        string GetStringValue();
    }


    public interface IForwardMovingTokenParser
    {
        /// <summary>
        /// moves forward and parses first token it can.  returns false if can't continue
        /// </summary>

        /// <returns>true if tokenized successfully</returns>
        /*Adfasd*/
        
        
        bool Parse(string text, int currentPosition, IToken currentToken,
            out int newPosition, out IToken newToken, out IForwardMovingTokenParser newParser);
    }

    public static class TokenParsingExtensions
    {
        /// <summary>
        /// tokenizes a string by a forward only parse
        /// </summary>
        /// <param name="text"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static List<IToken> Tokenize(this string text, IForwardMovingTokenParser parser)
        {
            List<IToken> rv = new List<IToken>();
            if (string.IsNullOrEmpty(text))
                return rv;
            if (parser == null)
                return rv;

            int pos = 0;
            int maxPos = text.Length - 1;
            IToken token = null;
            IForwardMovingTokenParser currentParser = parser;
            bool goodParse = true;

            while (goodParse && currentParser != null && pos > -1 && pos < maxPos)
            {
                var priorToken = token;
                var startPos = pos;

                goodParse = currentParser.Parse(text, pos, token, out pos, out token, out currentParser);
                if (goodParse)
                {
                    if (token != null)
                    {
                        token.PriorToken = priorToken;
                        token.HasStartEndPositions(startPos, pos);
        
                        rv.Add(token);
                    }
                }
            }
            return rv;
        }
    }
}
