using CuttingEdge.Conditions;
using Decoratid.Idioms.Stringing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.TokenParsing.HasStartEnd;

namespace Decoratid.Idioms.TokenParsing
{
    public static class ForwardMovingTokenizer
    {
        /// <summary>
        /// tokenizes a string by a forward only parse.  each tokenizer MUST provide the next tokenizer
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static List<IToken<T>> ForwardMovingTokenizeToCompletion<T>(this T[] rawData, object state, IForwardMovingTokenizer<T> parser)
        {
            List<IToken<T>> rv = new List<IToken<T>>();

            if (rawData == null)
                return rv;
            if (parser == null)
                return rv;

            int pos = 0;
            int maxPos = rawData.Length - 1;
            IToken<T> token = null;
            IForwardMovingTokenizer<T> currentParser = parser;
            IForwardMovingTokenizer<T> lastParser = null;
            bool goodParse = true;

            int counter = 0;

            while (goodParse && currentParser != null && pos > -1 && pos <= maxPos)
            {
                if (currentParser != null)
                    lastParser = currentParser;

                counter++;

                var priorToken = token;
                var startPos = pos;

                goodParse = currentParser.Parse(rawData, pos, state, token, out pos, out token, out currentParser);
                if (goodParse)
                {
                    if (token != null)
                    {
                        token.PriorToken = priorToken;
                        //decorate token with positional info
                        token = token.HasStartEnd(startPos, pos);

                        rv.Add(token);
                    }
                }
            }

            //if there's been a problem, kack 
            if (!goodParse)
                throw new LexingException(string.Format("tokenizing failed at pos {0} with tokenizer {1}", pos, lastParser.GetType()));

            return rv;
        }
        //public static List<IToken<T>> ForwardMovingTokenizeWithUnclassified<T>(this T[] rawData, object state, IForwardMovingTokenizer<T> parser)
        //{
        //    List<IToken<T>> rv = new List<IToken<T>>();



        //    return rv;
        //}
        /// <summary>
        /// tokenizes until it can't anymore
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rawData"></param>
        /// <param name="state"></param>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        public static List<IToken<T>> ForwardMovingTokenize<T>(this T[] rawData, object state, IForwardMovingTokenizer<T> parser, out int newPosition)
        {
            List<IToken<T>> rv = new List<IToken<T>>();
            newPosition = 0;

            if (rawData == null)
                return rv;

            if (parser == null)
                return rv;

            int pos = 0;
            int maxPos = rawData.Length - 1;
            IToken<T> token = null;
            IForwardMovingTokenizer<T> currentParser = parser;
            IForwardMovingTokenizer<T> lastParser = null;
            bool goodParse = true;

            int counter = 0;

            while (goodParse && currentParser != null && pos > -1 && pos <= maxPos)
            {
                if (currentParser != null)
                    lastParser = currentParser;

                counter++;

                var priorToken = token;
                var startPos = pos;

                goodParse = currentParser.Parse(rawData, pos, state, token, out pos, out token, out currentParser);
                if (goodParse)
                {
                    if (token != null)
                    {
                        token.PriorToken = priorToken;
                        //decorate token with positional info
                        token = token.HasStartEnd(startPos, pos);

                        rv.Add(token);
                    }
                }
            }

            newPosition = pos;
            return rv;
        }

    }
}
