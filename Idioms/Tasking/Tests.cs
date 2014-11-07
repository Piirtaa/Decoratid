using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Tasking
{
    public class JobTest : TestOf<Nothing>
    {
        public JobTest()
            : base(LogicOf<Nothing>.New((x) =>
            {




            })) 
        { 
        }
    }

    

}
