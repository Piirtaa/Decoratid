using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing.DotParenthesisComma
{
    /*
     *  "." -> "("
     * "("-> ",", ")"
     * ")" -> "."
     * 
     */

    /// <summary>
    /// At . seeking (
    /// </summary>
    public class ToOpenParenthesisParser : IForwardMovingTokenParser
    {
        #region IForwardMovingTokenParser
        public bool Parse(string text, int currentPosition, IToken currentToken, out int newPosition, out IToken newToken, out IForwardMovingTokenParser newParser)
        {
            //if we can't find a ( then we kack
            var idx = text.IndexOf("(", currentPosition);
            if (idx == -1)
            {
                newPosition = -1;
                newToken = null;
                newParser = null;
                return false;
            }
            else
            {
                var substring = text.Substring(currentPosition, idx - currentPosition);
                newPosition = idx + 1;
                newToken = Token.New(substring).HasDPCTokenType(DPCTokenType.Operation);//build up the token
                newParser = new ToCommaOrEndParenthesisParser();
                return true;
            }
        }
        #endregion
    }
    /// <summary>
    /// At ( or , seeking , or )
    /// </summary>
    public class ToCommaOrEndParenthesisParser : IForwardMovingTokenParser
    {
        #region IForwardMovingTokenParser
        public bool Parse(string text, int currentPosition, IToken currentToken, out int newPosition, out IToken newToken, out IForwardMovingTokenParser newParser)
        {
            //if we can't find a , or ) then we kack
            var idx = text.IndexOf(",", currentPosition);
            if (idx == -1)
            {
                idx = text.IndexOf(")", currentPosition);
                if (idx == -1)
                {
                    newPosition = -1;
                    newToken = null;
                    newParser = null;
                    return false;
                }

                var substring = text.Substring(currentPosition, idx - currentPosition);
                newPosition = idx + 1;
                newToken = Token.New(substring).HasDPCTokenType(DPCTokenType.Item);
                newParser = new ToDotParser();
                return true;
            }
            else
            {
                var substring = text.Substring(currentPosition, idx - currentPosition);
                newPosition = idx + 1;
                newToken = Token.New(substring).HasDPCTokenType(DPCTokenType.Item);
                newParser = new ToCommaOrEndParenthesisParser();
                return true;
            }
        }
        #endregion
    }
    /// <summary>
    /// At ) seeking .
    /// </summary>
    public class ToDotParser : IForwardMovingTokenParser
    {
        #region IForwardMovingTokenParser
        public bool Parse(string text, int currentPosition, IToken currentToken, out int newPosition, out IToken newToken, out IForwardMovingTokenParser newParser)
        {
            //if we can't find a . then we kack
            var idx = text.IndexOf(".", currentPosition);
            if (idx == -1)
            {
                newPosition = -1;
                newToken = null;
                newParser = null;
                return false;
            }
            else
            {
                var substring = text.Substring(currentPosition, idx - currentPosition);
                newPosition = idx + 1;
                newToken = Token.New(substring);//build up the token
                newParser = new ToOpenParenthesisParser();
                return true;
            }
        }
        #endregion
    }
}
