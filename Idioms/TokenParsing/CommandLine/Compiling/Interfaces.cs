using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing.CommandLine.Compiling
{
    /// <summary>
    /// common interface shared by arg tokens (eg. primitives) and UnitOfWork
    /// </summary>
    public interface ICanEval
    {
        object Evaluate();
    }
}
