using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Decoratid.Extensions;
using CuttingEdge.Conditions;

namespace Decoratid.Idioms.Throttling
{
    public class ThrottleTest : TestOf<Nothing>
    {
        public ThrottleTest()
            : base(LogicOf<Nothing>.New((x) =>
            {
                NaturalThrottle.New(1);

                int count = 0;
                var logic = Logic.New(() =>
                {
                    count++;
                    Thread.Sleep(2000);
                });

                var throttled = logic.Throttle(1);

                Action act1 = new Action(() => { throttled.Perform(); });
                Action act2 = new Action(() => { throttled.Perform(); });
                act1.EnqueueToThreadPool();
                act2.EnqueueToThreadPool();
                Thread.Sleep(1000);
                Condition.Requires(count).IsEqualTo(1);
                Thread.Sleep(1000);
                Condition.Requires(count).IsEqualTo(1);
                Thread.Sleep(2000);
                Condition.Requires(count).IsEqualTo(2);
            }))
        {
        }
    }

}
