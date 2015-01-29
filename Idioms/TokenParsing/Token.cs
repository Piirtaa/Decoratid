using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing
{
    public class Token : IToken
    {
        #region Declarations
        protected string _tokenString;
        #endregion

        #region Ctor
        public Token(string token)
        {
            this._tokenString = token;
        }
        #endregion

        #region Fluent Static
        public static Token New(string token)
        {
            return new Token(token);
        }
        #endregion

        #region IToken
        public IToken PriorToken { get; set; }
        public string GetStringValue()
        {
            return this._tokenString;
        }
        #endregion
    }
}
