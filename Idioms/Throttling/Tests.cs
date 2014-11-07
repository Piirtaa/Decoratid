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

namespace Decoratid.Idioms.Throttling
{
    public class ThrottleTest : TestOf<Nothing>
    {
        public ThrottleTest()
            : base(LogicOf<Nothing>.New((x) =>
            {
                NaturalThrottle.New(1);

                var logic = Logic.New(() =>
                {
                    Thread.Sleep(1000);
                });

                int count = 0;


                var throttled = logic.Throttle(1);

                //throttled.Perform(

            }))
        {
        }
    }

    public class ConditionTest : TestOf<ICondition>
    {
        public ConditionTest()
            : base(LogicOf<ICondition>.New((x) =>
            {
                var throttled = x.Throttle(1);



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
