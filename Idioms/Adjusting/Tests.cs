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
using CuttingEdge.Conditions;

namespace Decoratid.Idioms.Adjusting
{
    public class ConditionTest : TestOf<ICondition>
    {
        public ConditionTest()
            : base(LogicOf<ICondition>.New((x) =>
            {
                //tests "adjustment" idiom for Conditions
                //we adjust by ANDing a switch that we turn off and on

                //get the old eval.  we'll be expecting this value with the switch on
                var oldEval = x.Evaluate();

                //the adjustment is an AND with a conditional bool that we switch on and off
                var switchCond = StrategizedConditionOf<bool>.New((cond) => 
                { 
                    return new bool?(cond); 
                }).AddContext(false);

                //apply the adjustment
                var newX = x.Adjust((cond) =>
                {
                    return cond.And(switchCond);
                });

                //if we evaluate the adjusted condition it should eval to false due to the switch condition
                var adjustedEval = newX.Evaluate();
                Condition.Requires(adjustedEval).IsEqualTo(false);

                //switch on
                switchCond.Context = true;
                adjustedEval = newX.Evaluate();
                Condition.Requires(adjustedEval).IsEqualTo(oldEval);
            }))
        {
        }
    }

    public class ValueOfTest : TestOf<IValueOf<string>>
    {
        public ValueOfTest()
            : base(LogicOf<IValueOf<string>>.New((x) =>
            {
                //tests "adjustment" idiom for ValueOf
                //the adjustment appends a value "..Bro..." to the string
                string appendText = "..Bro...";

                var oldVal = x.GetValue();

                var adjusted = x.Adjust((old) =>
                {
                    return old + appendText;
                });

                var newVal = adjusted.GetValue();
                Condition.Requires(newVal).EndsWith(appendText);

            }))
        {
        }
    }

    public class LogicTest : TestOf<ILogic>
    {
        public LogicTest()
            : base(LogicOf<ILogic>.New((x) =>
            {
                //tests "adjustment" idiom for logic
                //the adjustment changes the value of a variable
                var switchVar = false;

                var adjusted = x.Adjust((old) =>
                {
                    var adjustedLogic = Logic.New(() =>
                    {
                        old.Perform();
                        switchVar = true;
                    });
                    return adjustedLogic;
                });

                adjusted.Perform();
                Condition.Requires(switchVar).IsTrue();
            }))
        {
        }
    }
}
