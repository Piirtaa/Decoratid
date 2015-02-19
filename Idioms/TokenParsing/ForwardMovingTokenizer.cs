using CuttingEdge.Conditions;
using Decoratid.Idioms.Stringing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing
{
    public static class  ForwardMovingTokenizer
    {
        /// <summary>
        /// tokenizes a string by a forward only parse.  each tokenizer MUST provide the next tokenizer
        /// </summary>
        /// <param name="text"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static List<IToken> ForwardMovingTokenize(this string text, object state, IForwardMovingTokenizer parser)
        {
            List<IToken> rv = new List<IToken>();
            if (string.IsNullOrEmpty(text))
                return rv;
            if (parser == null)
                return rv;

            int pos = 0;
            int maxPos = text.Length - 1;
            IToken token = null;
            IForwardMovingTokenizer currentParser = parser;
            bool goodParse = true;

            int counter= 0;

            while (goodParse && currentParser != null && pos > -1 && pos <= maxPos)
            {
                counter++;

                var priorToken = token;
                var startPos = pos;

                goodParse = currentParser.Parse(text, pos, state, token, out pos, out token, out currentParser);
                if (goodParse)
                {
                    if (token != null)
                    {
                        token.PriorToken = priorToken;
                        //decorate token with positional info
                        token = token.HasStartEndPositions(startPos, pos);

                        rv.Add(token);
                    }
                }
            }

            //if there's been a problem, kack 
            if (!goodParse)
                throw new LexingException(string.Format("tokenizing failed at pos {0} with tokenizer {1}", pos, currentParser.GetType()));
            
            return rv;
        }
    }
}
