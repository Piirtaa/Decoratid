using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Extensions;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Eventing
{
    public class ConditionTest : TestOf<ICondition>
    {
        public ConditionTest()
            : base(LogicOf<ICondition>.New((x) =>
            {
                bool checkVal = false;
                var newX = x.Eventing();
                newX.Evaluated += delegate(object sender, EventArgOf<ICondition> e)
                {
                    checkVal = true;
                };
                newX.Evaluate();
                Thread.Sleep(50);
                Condition.Requires(checkVal).IsTrue();
            })) 
        { 
        }
    }

    public class ValueOfTest<T> : TestOf<IValueOf<T>>
    {
        public ValueOfTest()
            : base(LogicOf<IValueOf<T>>.New((x) =>
            {
                bool checkVal = false;
                var newX = x.Eventing();
                newX.Evaluated += delegate(object sender, EventArgOf<IValueOf<T>> e)
                {
                    checkVal = true;
                };
                newX.GetValue();
                Thread.Sleep(50);
                Condition.Requires(checkVal).IsTrue();


            }))
        {
        }
    }

    public class LogicTest : TestOf<ILogic>
    {
        public LogicTest()
            : base(LogicOf<ILogic>.New((x) =>
            {
                bool checkVal = false;
                var newX = x.Eventing();
                newX.Evaluated += delegate(object sender, EventArgOf<ILogic> e)
                {
                    checkVal = true;
                };
                newX.Perform();
                Thread.Sleep(50);
                Condition.Requires(checkVal).IsTrue();
            }))
        {
        }
    }
}
