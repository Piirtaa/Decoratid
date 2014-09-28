using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Expiring
{
    public class NaturalFalseExpirable : IExpirable
    {
        public bool IsExpired()
        {
            return false;
        }
    }

    public class NaturalTrueExpirable : IExpirable
    {
        public bool IsExpired()
        {
            return true;
        }
    }
    /// <summary>
    /// throws InvalidOperationException
    /// </summary>
    public class NaturalUndefinedExpirable : IExpirable
    {
        public bool IsExpired()
        {
            throw new InvalidOperationException ();
        }
    }
}
