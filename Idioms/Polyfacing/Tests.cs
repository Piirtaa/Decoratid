using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
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
                var pf = x.Polyfacing<ICondition>();
                var face = pf.As<ICondition>();
                var decorated = face.GetDecorated();
                Condition.Requires(object.ReferenceEquals(decorated, x)).IsTrue();

            })) 
        { 
        }
    }

    public class ValueOfTest : TestOf<IValueOf<string>>
    {
        public ValueOfTest()
            : base(LogicOf<IValueOf<string>>.New((x) =>
            {
                var pf = x.Polyfacing<string, IValueOf<string>>();
                var face = pf.As<IValueOf<string>>();
                var decorated = face.GetDecorated();
                Condition.Requires(object.ReferenceEquals(decorated, x)).IsTrue();

            }))
        {
        }
    }

    public class LogicTest : TestOf<ILogic>
    {
        public LogicTest()
            : base(LogicOf<ILogic>.New((x) =>
            {
                var pf = x.Polyfacing<ILogic>();
                var face = pf.As<ILogic>();
                var decorated = face.GetDecorated();
                Condition.Requires(object.ReferenceEquals(decorated, x)).IsTrue();

            }))
        {
        }
    }
}
