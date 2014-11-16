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

namespace Decoratid.Idioms.Counting
{
    public class ConditionTest : TestOf<ICondition>
    {
        public ConditionTest()
            : base(LogicOf<ICondition>.New((x) =>
            {
                var newX = x.Counted();
                Condition.Requires(newX.Counter.Current).IsEqualTo(0);
                newX.Evaluate();
                Condition.Requires(newX.Counter.Current).IsEqualTo(1);
    
            })) 
        { 
        }
    }

    public class ValueOfTest : TestOf<IValueOf<string>>
    {
        public ValueOfTest()
            : base(LogicOf<IValueOf<string>>.New((x) =>
            {
                var newX = x.Counted();
                Condition.Requires(newX.Counter.Current).IsEqualTo(0);
                newX.GetValue();
                Condition.Requires(newX.Counter.Current).IsEqualTo(1);
            }))
        {
        }
    }

    public class LogicTest : TestOf<ILogic>
    {
        public LogicTest()
            : base(LogicOf<ILogic>.New((x) =>
            {
                var newX = x.Counted();
                Condition.Requires(newX.Counter.Current).IsEqualTo(0);
                newX.Perform();
                Condition.Requires(newX.Counter.Current).IsEqualTo(1);

            }))
        {
        }
    }
}
