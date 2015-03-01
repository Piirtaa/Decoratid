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

namespace Decoratid.Idioms.TokenParsing.CommandLine.Lexing
{
    public class CLLexingTest : TestOf<Nothing>
    {
        public CLLexingTest()
            : base(LogicOf<Nothing>.New((x) =>
            {
                List<string> commands = new List<string>()
                {
                    "@store.search(#ness.IsThing('x','y',7))",
                    "7.AsId().HasName('name')"
                };
                
                var config = CLConfig.New();

                var tokens = CommandLineLexer.ForwardMovingTokenize(config, commands[0]);

                //Condition.Requires(cmds).HasLength(2);
                //Condition.Requires(cmds[1].OperationToken.TokenString).IsEqualTo("dosomethingelse");
                //Condition.Requires(cmds[0].ArgTokens[3].TokenString).IsEqualTo("");
                Debug.WriteLine("");
            })) 
        { 
        }
    }

}
