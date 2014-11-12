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

namespace Decoratid.Idioms.ObjectGraphing
{
    public class ConditionTest : TestOf<Nothing>
    {
        public ConditionTest()
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
                var readable = LengthEncoder.MakeReadable(objState1);
                var graph = Graph.Parse(objState1, ValueManagerChainOfResponsibility.NewDefault());
                var readable2 = GraphingUtil.ConvertToXML(graph).ToString();

                var obj2 = objState1.GraphDeserializeWithDefaults() as HasVersionDecoration;
                Condition.Requires(obj2.Version).IsEqualTo("v");
                Condition.Requires(obj2.FindDecoratorOf<HasRandomStringDecoration>(true).RandomString).IsEqualTo("blah");
                Condition.Requires(obj2.FindDecoratorOf<HasIPDecoration>(true).IPAddress.ToString()).IsEqualTo(ip.ToString());
                Condition.Requires(obj2.FindDecoratorOf<HasMachineNameDecoration>(true).MachineName).IsEqualTo(localMachineName);
                Condition.Requires(obj2.FindDecoratorOf<HasDateLastTouchedDecoration>(true).DateLastTouched.ToString()).IsEqualTo(lastTouchedDate.ToUniversalTime().ToString());
                Condition.Requires(obj2.FindDecoratorOf<HasDateCreatedDecoration>(true).DateCreated.ToString()).IsEqualTo(now.ToUniversalTime().ToString());
                Condition.Requires(obj2.FindDecoratorOf<HasGUIDDecoration>(true).GUID).IsEqualTo(guid);
                Condition.Requires(obj2.Id.ToString()).IsEqualTo("id");

                hasV.Version = "v2";
                var objState2 = hasV.GraphSerializeWithDefaults();
                var obj3 = objState1.GraphDeserializeWithDefaults() as HasVersionDecoration;
                Condition.Requires(obj3.Version).IsEqualTo("v2");

            })) 
        { 
        }
    }

    public class ValueOfTest<T> : TestOf<IValueOf<T>>
    {
        public ValueOfTest()
            : base(LogicOf<IValueOf<T>>.New((x) =>
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
