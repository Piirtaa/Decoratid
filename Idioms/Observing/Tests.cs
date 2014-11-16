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

namespace Decoratid.Idioms.Observing
{
    public class ConditionTest : TestOf<ICondition>
    {
        public ConditionTest()
            : base(LogicOf<ICondition>.New((x) =>
            {
                var data = "data";

                var observer = x.Observe(LogicOf<ICondition>.New((o) =>
                {
                    data = "data1";
                }), null);

                Condition.Requires(data).IsEqualTo("data");
                observer.Evaluate();
                Condition.Requires(data).IsEqualTo("data1");
     
            })) 
        { 
        }
    }

    public class ValueOfTest : TestOf<IValueOf<string>>
    {
        public ValueOfTest()
            : base(LogicOf<IValueOf<string>>.New((x) =>
            {
                var data = "data";

                var observer = x.Observe(LogicOf<IValueOf<string>>.New((o) =>
                {
                    data = "data1";
                }), null);

                Condition.Requires(data).IsEqualTo("data");
                observer.GetValue();
                Condition.Requires(data).IsEqualTo("data1");

            }))
        {
        }
    }

    public class LogicTest : TestOf<ILogic>
    {
        public LogicTest()
            : base(LogicOf<ILogic>.New((x) =>
            {
                var data = "data";

                var observer = x.Observe(LogicOf<ILogic>.New((o) =>
                {
                    data = "data1";
                }), null);

                Condition.Requires(data).IsEqualTo("data");
                observer.Perform();
                Condition.Requires(data).IsEqualTo("data1");
            }))
        {
        }
    }
}
