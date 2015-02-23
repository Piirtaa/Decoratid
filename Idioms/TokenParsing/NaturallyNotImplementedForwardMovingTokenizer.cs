using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing
{
    public class NaturallyNotImplementedForwardMovingTokenizer<T> : IForwardMovingTokenizer<T>
    {
        #region Ctor
        public NaturallyNotImplementedForwardMovingTokenizer() { }
        #endregion

        #region Fluent Static
        public static NaturallyNotImplementedForwardMovingTokenizer<T> New() { return new NaturallyNotImplementedForwardMovingTokenizer<T>(); }
        #endregion

        #region IForwardMovingTokenizer
        public bool Parse(T[] dataToTokenize, int currentPosition, object state, IToken<T> currentToken,
            out int newPosition, out IToken<T> newToken, out IForwardMovingTokenizer<T> newParser)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
