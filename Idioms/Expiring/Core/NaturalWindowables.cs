using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Expiring.Core
{
    public class NaturalTrueWindowable : IWindowable
    {
        public bool IsInWindow(DateTime dt)
        {
            return true;
        }
    }

    public class NaturalFalseWindowable : IWindowable
    {
        public bool IsInWindow(DateTime dt)
        {
            return false;
        }
    }

    /// <summary>
    /// throws InvalidOperationException
    /// </summary>
    public class NaturalUndefinedWindowable : IExpirable
    {
        public bool IsInWindow()
        {
            throw new InvalidOperationException();
        }
    }
}
