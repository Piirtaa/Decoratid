using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TokenParsing.DotParenthesisComma
{
    public class DPCParseTest : TestOf<Nothing>
    {
        public DPCParseTest()
            : base(LogicOf<Nothing>.New((x) =>
            {
                string commandtoparse = ".dosomething(11.0,15,asdkljlkjmjh,).dosomethingelse(aaadsfasdf)";

                var cmds = commandtoparse.TokenizeToDPCOperations();

                Debug.WriteLine("");
            })) 
        { 
        }
    }

}
