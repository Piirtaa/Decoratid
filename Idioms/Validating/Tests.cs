using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Contextual;

namespace Decoratid.Idioms.Validating
{
    public class ConditionTest : TestOf<ICondition>
    {
        public ConditionTest()
            : base(LogicOf<ICondition>.New((x) =>
            {
                var cond = StrategizedConditionOf<bool>.New((o) => { return o; }).AddContext(false);
                var valid = x.KackUnless(cond);

                bool isValid = false;
                try
                {
                    valid.Evaluate();
                }
                catch
                {
                    isValid = true;
                }
                Condition.Requires(isValid).IsTrue();

                cond.Context = true;
                valid.Evaluate();
   
            })) 
        { 
        }
    }

    public class ValueOfTest<T> : TestOf<IValueOf<T>>
    {
        public ValueOfTest()
            : base(LogicOf<IValueOf<T>>.New((x) =>
            {
                var cond = StrategizedConditionOf<bool>.New((o) => { return o; }).AddContext(false);
                var valid = x.KackUnless(cond);

                bool isValid = false;
                try
                {
                    valid.GetValue();
                }
                catch
                {
                    isValid = true;
                }
                Condition.Requires(isValid).IsTrue();

                cond.Context = true;
                valid.GetValue();

            }))
        {
        }
    }

    public class LogicTest : TestOf<ILogic>
    {
        public LogicTest()
            : base(LogicOf<ILogic>.New((x) =>
            {
                var cond = StrategizedConditionOf<bool>.New((o) => { return o; }).AddContext(false);
                var valid = x.KackUnless(cond);

                bool isValid = false;
                try
                {
                    valid.Perform();
                }
                catch
                {
                    isValid = true;
                }
                Condition.Requires(isValid).IsTrue();

                cond.Context = true;
                valid.Perform();

            }))
        {
        }
    }
}
