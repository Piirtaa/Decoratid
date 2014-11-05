using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Communicating.Socketing
{
    public class HostTest : TestOf<Host>
    {
        public HostTest()
            : base(LogicOf<Host>.New((x) =>
            {
 
                //give the host echo logic
                x.HasLogic(LogicOfTo<string, string>.New((req) => { return req; }));
                //start the host
                x.Start();

                //build a client
                var client = Client.New(x.EndPoint);

                //send some data
                var reqDat = "Hello world";
                var dat = client.Send(reqDat);
                Condition.Requires(dat).IsEqualTo(reqDat);
            })) 
        { 
        }
    }

    
}
