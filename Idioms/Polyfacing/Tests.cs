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

namespace Decoratid.Idioms.Polyfacing
{
    public class ConditionTest : TestOf<ICondition>
    {
        public ConditionTest()
            : base(LogicOf<ICondition>.New((x) =>
            {
                var pf = x.Polyfacing();
                var face = pf.As<ICondition>();
                Condition.Requires(object.ReferenceEquals(face, x)).IsTrue();

            })) 
        { 
        }
    }

    public class ValueOfTest<T> : TestOf<IValueOf<T>>
    {
        public ValueOfTest()
            : base(LogicOf<IValueOf<T>>.New((x) =>
            {
                var pf = x.Polyfacing();
                var face = pf.As<IValueOf<T>>();
                Condition.Requires(object.ReferenceEquals(face, x)).IsTrue();

            }))
        {
        }
    }

    public class LogicTest : TestOf<ILogic>
    {
        public LogicTest()
            : base(LogicOf<ILogic>.New((x) =>
            {
                var pf = x.Polyfacing();
                var face = pf.As<ILogic>();
                Condition.Requires(object.ReferenceEquals(face, x)).IsTrue();

            }))
        {
        }
    }
}
