using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing.CommandLine.Compiling
{
    [Serializable]
    public class CommandLineCompilationException : ApplicationException
    {
        public CommandLineCompilationException() : base() { }
        public CommandLineCompilationException(String message) : base(message) { }
        public CommandLineCompilationException(string message, Exception ex) : base(message, ex) { }

    }
}
