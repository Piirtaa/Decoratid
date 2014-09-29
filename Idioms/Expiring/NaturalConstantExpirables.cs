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

        public static NaturalFalseExpirable New()
        {
            return new NaturalFalseExpirable();
        }
    }

    public class NaturalTrueExpirable : IExpirable
    {
        public bool IsExpired()
        {
            return true;
        }

        public static NaturalTrueExpirable New()
        {
            return new NaturalTrueExpirable();
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

        public static NaturalUndefinedExpirable New()
        {
            return new NaturalUndefinedExpirable();
        }
    }
}
