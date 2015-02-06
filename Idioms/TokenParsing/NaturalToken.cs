using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing
{
    /// <summary>
    /// most basic token.  just keeps a string value
    /// </summary>
    public class NaturalToken : IToken
    {
        #region Declarations
        protected string _tokenString;
        #endregion

        #region Ctor
        public NaturalToken(string token)
        {
            this._tokenString = token;
        }
        #endregion

        #region Fluent Static
        public static NaturalToken New(string token)
        {
            return new NaturalToken(token);
        }
        #endregion

        #region IToken
        public IToken PriorToken { get; set; }
        public string TokenString
        {
            get
            {
                return this._tokenString;
            }
        }
        #endregion
    }
}
