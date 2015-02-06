using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing.StateMachined
{
    /*
     * 
     */

    public interface IHasPrefix
    {
        string Prefix { get; }
    }
    public interface IHasSuffix
    {
        string Suffix { get; }
    }

    /// <summary>
    /// only parses data from prefix to suffix
    /// </summary>
    public class ToFromDelimiterParser : IForwardMovingTokenParser, IHasPrefix,IHasSuffix, IHasId<string>
    {
        #region Ctor
        public ToFromDelimiterParser(string prefix, string suffix)
        {
            this.Prefix = prefix;
            this.Suffix = suffix;
        }
        #endregion

        #region Fluent Static
        public static ToFromDelimiterParser New(string prefix, string suffix)
        {
            return new ToFromDelimiterParser(prefix, suffix);
        }
        #endregion

        #region IHasPrefix
        public string Prefix { get; private set; }
        #endregion

        #region IHasSuffix
        public string Suffix { get; private set; }
        #endregion

        #region IHasId
        public string Id { get { return this.Prefix + "|" + this.Suffix; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region IForwardMovingTokenParser
        public bool Parse(string text, int currentPosition, IToken currentToken, out int newPosition, out IToken newToken, out IForwardMovingTokenParser newParser)
        {
            //if we can't find the suffix, we kack
            var idx = text.IndexOf(this.Suffix, currentPosition);
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
                newToken = NaturalToken.New(substring);//build up the token
                newParser = new ToOpenParenthesisParser();
                return true;
            }
        }
        #endregion
    }
}
