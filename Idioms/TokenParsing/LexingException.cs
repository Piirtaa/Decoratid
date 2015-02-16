using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing
{
    [Serializable]
    public class LexingException : ApplicationException
    {
        public LexingException() : base() { }
        public LexingException(String message) : base(message) { }
        public LexingException(string message, Exception ex) : base(message, ex) { }

    }
}
