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
        /// tokenizes a string by a forward only parse
        /// </summary>
        /// <param name="text"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public static List<IToken> Tokenize(string text, object state, IForwardMovingTokenizer parser)
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

            while (goodParse && currentParser != null && pos > -1 && pos < maxPos)
            {
                var priorToken = token;
                var startPos = pos;

                goodParse = currentParser.Parse(text, pos, state, token, out pos, out token, out currentParser);
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
