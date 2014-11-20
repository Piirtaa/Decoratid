using Decoratid.Idioms.Communicating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Core.Conditional;
using Decoratid.Core.Contextual;
using Decoratid.Core.Storing;

namespace Decoratid.Idioms.Testing
{
    /// <summary>
    /// builds mocks of core Things
    /// </summary>
    public static class MockBuilder
    {
        public static IHasId BuildMockIHasId(string id)
        {
            //build an object up
            var hasId = id.BuildAsId();

            var guid = Guid.NewGuid();
            var hasGuid = hasId.HasGUID(guid);
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

            return hasV;
        }

        public static ILogic BuildMockLogic()
        {
            var logic = Logic.New(() => { Console.WriteLine("hello world"); });
            return logic;
        }
        public static IValueOf<IHasId> BuildMockValueOf(string id)
        {
            return BuildMockIHasId(id).AsNaturalValue();
        }
        public static ICondition BuildMockCondition()
        {
            return IsFalse.New();
        }

        public static IStore BuildMockStore()
        {
            return NamedNaturalInMemoryStore.New("test store");
        }
    }
}
