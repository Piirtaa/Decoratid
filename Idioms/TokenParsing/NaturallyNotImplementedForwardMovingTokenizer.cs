using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing
{
    public class NaturallyNotImplementedForwardMovingTokenizer : IForwardMovingTokenizer
    {
        #region Ctor
        public NaturallyNotImplementedForwardMovingTokenizer() { }
        #endregion

        #region Fluent Static
        public static NaturallyNotImplementedForwardMovingTokenizer New() { return new NaturallyNotImplementedForwardMovingTokenizer(); }
        #endregion

        #region IForwardMovingTokenizer
        public bool Parse(string text, int currentPosition, object state, IToken currentToken, out int newPosition, out IToken newToken, out IForwardMovingTokenizer newParser)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
