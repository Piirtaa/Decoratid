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
using Decoratid.Idioms.Identifying;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.Communicating;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.ObjectGraphing.Values;
using Decoratid.Extensions;
using System.Diagnostics;
using Decoratid.Core.Decorating;

namespace Decoratid.Idioms.ObjectGraphing
{
    public class GraphingTest : TestOf<Nothing>
    {
        public GraphingTest()
            : base(LogicOf<Nothing>.New((x) =>
            {

                //build an object up
                var id = "id".BuildAsId();

                var guid = Guid.NewGuid();
                var hasGuid = id.HasGUID(guid);
                var now = DateTime.Now;
                var hasDateCreated = hasGuid.HasDateCreated(now);
                var lastTouchedDate = DateTime.Now;
                var hasLastTouched = hasDateCreated.HasDateLastTouched(lastTouchedDate);
                var localMachineName = NetUtil.GetLocalMachineName();
                var hasLM = hasLastTouched.HasLocalMachineName();
                var ip = NetUtil.GetLocalIPAddresses().First();
                var hasIP = hasLM.HasIP(ip);
                var hasRS = hasIP.HasRandomString("blah");
                var hasV = hasRS.HasVersion("v");
      
                //graph it
                var objState1 = hasV.GraphSerializeWithDefaults();
                var readable = LengthEncoder.MakeReadable(objState1, "\t");
                readable.WithEach(i => { Debug.WriteLine(i); });


                var graph = Graph.Parse(objState1, ValueManagerChainOfResponsibility.NewDefault());
                var readable2 = GraphingUtil.ConvertToXML(graph).ToString();

                var obj2 = objState1.GraphDeserializeWithDefaults() as HasVersionDecoration;
                Condition.Requires(obj2.Version).IsEqualTo("v");
                Condition.Requires(obj2.FindDecoration<HasRandomStringDecoration>(true).RandomString).IsEqualTo("blah");
                Condition.Requires(obj2.FindDecoration<HasIPDecoration>(true).IPAddress.ToString()).IsEqualTo(ip.ToString());
                Condition.Requires(obj2.FindDecoration<HasMachineNameDecoration>(true).MachineName).IsEqualTo(localMachineName);
                Condition.Requires(obj2.FindDecoration<HasDateLastTouchedDecoration>(true).DateLastTouched.ToString()).IsEqualTo(lastTouchedDate.ToUniversalTime().ToString());
                Condition.Requires(obj2.FindDecoration<HasDateCreatedDecoration>(true).DateCreated.ToString()).IsEqualTo(now.ToUniversalTime().ToString());
                Condition.Requires(obj2.FindDecoration<HasGUIDDecoration>(true).GUID).IsEqualTo(guid);
                Condition.Requires(obj2.Id.ToString()).IsEqualTo("id");

                hasV.Version = "v2";
                var objState2 = hasV.GraphSerializeWithDefaults();
                var obj3 = objState2.GraphDeserializeWithDefaults() as HasVersionDecoration;
                Condition.Requires(obj3.Version).IsEqualTo("v2");

            })) 
        { 
        }
    }

    public class ValueOfTest : TestOf<IValueOf<string>>
    {
        public ValueOfTest()
            : base(LogicOf<IValueOf<string>>.New((x) =>
            {
                //TESTS HERE




            }))
        {
        }
    }

    public class LogicTest : TestOf<ILogic>
    {
        public LogicTest()
            : base(LogicOf<ILogic>.New((x) =>
            {
                //TESTS HERE




            }))
        {
        }
    }
}
