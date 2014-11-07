using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Communicating;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Identifying
{
    public class IdentifyingTest : TestOf<IHasId>
    {
        public IdentifyingTest()
            : base(LogicOf<IHasId>.New((x) =>
            {
                //try out all the decorations
                var guid = Guid.NewGuid();
                var hasGuid = x.HasGUID(guid);
                Condition.Requires(hasGuid.GUID).IsEqualTo(guid);

                var now = DateTime.Now;
                var hasDateCreated = hasGuid.HasDateCreated(now);
                Condition.Requires(hasDateCreated.DateCreated).IsEqualTo(now);

                var lastTouchedDate = DateTime.Now;
                var hasLastTouched = hasGuid.HasDateLastTouched(lastTouchedDate);
                Condition.Requires(hasLastTouched.DateLastTouched).IsEqualTo(lastTouchedDate);

                var localMachineName = NetUtil.GetLocalMachineName();
                var hasLM = hasGuid.HasLocalMachineName();
                Condition.Requires(hasLM.MachineName).IsEqualTo(localMachineName);

                var ip = NetUtil.GetLocalIPAddresses().First();
                var hasIP = hasGuid.HasIP(ip);
                Condition.Requires(hasIP.IPAddress.ToString()).IsEqualTo(ip.ToString());

                var hasRS = hasGuid.HasRandomString("blah");
                Condition.Requires(hasRS.RandomString).IsEqualTo("blah");

                var hasV = hasGuid.HasVersion("v");
                Condition.Requires(hasV.Version).IsEqualTo("v");
            })) 
        { 
        }
    }

}
