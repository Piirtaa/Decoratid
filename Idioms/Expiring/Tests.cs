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
using Decoratid.Idioms.ErrorCatching;

namespace Decoratid.Idioms.Expiring
{
    public class ConditionTest : TestOf<ICondition>
    {
        public ConditionTest()
            : base(LogicOf<ICondition>.New((x) =>
            {
                var expiry = DateTime.Now.AddSeconds(5);
                var newX = x.HasExpirable();
                newX.ExpiresWhen(StrategizedCondition.New(() => { return DateTime.Now < expiry; }));
                newX.Evaluate();
                Thread.Sleep(6000);

                var trapX = newX.Traps();
                trapX.Evaluate();

            })) 
        { 
        }
    }

    public class ValueOfTest<T> : TestOf<IValueOf<T>>
    {
        public ValueOfTest()
            : base(LogicOf<IValueOf<T>>.New((x) =>
            {
                var expiry = DateTime.Now.AddSeconds(5);
                var newX = x.HasExpirable();
                newX.ExpiresWhen(StrategizedCondition.New(() => { return DateTime.Now < expiry; }));
                newX.GetValue();
                Thread.Sleep(6000);

                var trapX = newX.Traps();
                trapX.GetValue();

            }))
        {
        }
    }

    public class LogicTest : TestOf<ILogic>
    {
        public LogicTest()
            : base(LogicOf<ILogic>.New((x) =>
            {
                var expiry = DateTime.Now.AddSeconds(5);
                var newX = x.HasExpirable();
                newX.ExpiresWhen(StrategizedCondition.New(() => { return DateTime.Now < expiry; }));
                newX.Perform();
                Thread.Sleep(6000);

                var trapX = newX.Traps();
                trapX.Perform();

            }))
        {
        }
    }
}
