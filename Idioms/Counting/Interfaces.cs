using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Counting
{
    public interface IHasCounter
    {
        Counter Counter { get; }
    }
}
