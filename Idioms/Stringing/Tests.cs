using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Stringing
{
    public class StringableTest : TestOf<IStringable>
    {
        public StringableTest()
            : base(LogicOf<IStringable>.New((x) =>
            {
     



            })) 
        { 
        }
    }

}
