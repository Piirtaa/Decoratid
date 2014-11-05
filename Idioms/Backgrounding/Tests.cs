using CuttingEdge.Conditions;
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

namespace Decoratid.Idioms.Backgrounding
{
    public class ConditionTest : TestOf<ICondition>
    {
        public ConditionTest()
            : base(LogicOf<ICondition>.New((x) =>
            {
                //give this thing polling, and have the polling job turn a switch on
                bool switchVal = false;

                var newX = x.Polls();
                newX.SetBackgroundAction(LogicOf<ICondition>.New((thing) =>
                {
                    switchVal = true;
                }), 1000);

                //wait 2 seconds and the switch should be true
                Thread.Sleep(2000);
                Condition.Requires(switchVal).IsTrue();
            })) 
        { 
        }
    }

    public class ValueOfTest<T> : TestOf<IValueOf<T>>
    {
        public ValueOfTest()
            : base(LogicOf<IValueOf<T>>.New((x) =>
            {
                //give this thing polling, and have the polling job turn a switch on
                bool switchVal = false;

                var newX = x.Polls();
                newX.SetBackgroundAction(LogicOf<IValueOf<T>>.New((thing) =>
                {
                    switchVal = true;
                }), 1000);

                //wait 2 seconds and the switch should be true
                Thread.Sleep(2000);
                Condition.Requires(switchVal).IsTrue();

            }))
        {
        }
    }

    public class LogicTest : TestOf<ILogic>
    {
        public LogicTest()
            : base(LogicOf<ILogic>.New((x) =>
            {
                //give this thing polling, and have the polling job turn a switch on
                bool switchVal = false;

                var newX = x.Polls();
                newX.SetBackgroundAction(LogicOf<ILogic>.New((thing) =>
                {
                    switchVal = true;
                }), 1000);

                //wait 2 seconds and the switch should be true
                Thread.Sleep(2000);
                Condition.Requires(switchVal).IsTrue();

            }))
        {
        }
    }
}
