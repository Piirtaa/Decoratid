using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Backgrounding
{
    public interface IHasBackgroundHost
    {
        BackgroundHost Background { get; }
    }



}
