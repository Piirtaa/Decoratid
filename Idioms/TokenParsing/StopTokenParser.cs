//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Decoratid.Idioms.TokenParsing
//{
//    /// <summary>
//    /// this parser will stop the parsing process
//    /// </summary>
//    public class StopTokenParser : IForwardMovingTokenParser
//    {
//        #region Ctor
//        public StopTokenParser()
//        {

//        }
//        #endregion

//        #region Static Fluent
//        public static StopTokenParser New()
//        {
//            return new StopTokenParser();
//        }
//        #endregion

//        #region IForwardMovingTokenParser
//        public bool Parse(string text, int position, 
//            out int newPosition, out IToken token, out IForwardMovingTokenParser nextParser)
//        {
//            newPosition = -1;
//            token = null;
//            nextParser = null;
//            return false;
//        }
//        #endregion
//    }
//}
