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
    public class NaturalToken<T> : IToken<T>
    {
        #region Declarations
        protected T[] _tokenData;
        #endregion

        #region Ctor
        public NaturalToken(T[] tokenData)
        {
            this._tokenData = tokenData;
        }
        #endregion

        #region Fluent Static
        public static NaturalToken<T> New(T[] tokenData)
        {
            return new NaturalToken<T>(tokenData);
        }
        #endregion

        #region IToken
        public IToken<T> PriorToken { get; set; }
        public T[] TokenData
        {
            get
            {
                return this._tokenData;
            }
        }
        #endregion
    }
}
