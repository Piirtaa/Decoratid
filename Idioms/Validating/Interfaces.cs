using Decoratid.Idioms.Core.Conditional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Validating
{
    /// <summary>
    /// validates operations. kacks if invalid
    /// </summary>
    public interface IHasValidator
    {
        ICondition IsValidCondition { get; }
    }
}
