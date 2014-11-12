//using Decoratid.Core.Conditional;
//using Decoratid.Core.Conditional.Of;
//using Decoratid.Core.Logical;
//using Decoratid.Core.ValueOfing;
//using Decoratid.Idioms.Testing;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Decoratid.Core.Contextual;
//using CuttingEdge.Conditions;
//using Decoratid.Idioms.ObjectGraphing;

//namespace Decoratid.Idioms.ConditionalWaiting
//{
//    public class WaiterTest : TestOf<Nothing>
//    {
//        public WaiterTest()
//            : base(LogicOf<Nothing>.New((x) =>
//            {
//                var flagDate = DateTime.Now.AddSeconds(5);
//                ICondition cond = StrategizedCondition.New(() => { return DateTime.Now < flagDate; });
//                ICondition expCond = StrategizedCondition.New(() => { return DateTime.Now > flagDate.AddSeconds(1); });
//                var waiter = ConditionalWaiter.New(cond, expCond);
//                waiter.WaitAround();
//            }))
//        {
//        }
//    }

//    public class ConditionTest : TestOf<ICondition>
//    {
//        public ConditionTest()
//            : base(LogicOf<ICondition>.New((x) =>
//            {
//                var flagDate = DateTime.Now.AddSeconds(5);
//                ICondition cond = StrategizedCondition.New(() => { return DateTime.Now < flagDate; });
//                ICondition expCond = StrategizedCondition.New(() => { return DateTime.Now > flagDate.AddSeconds(1); });

//                var newX = x.WaitUntil(cond, expCond);
//                var oldVal = x.Evaluate();
//                var newVal = newX.Evaluate();
//                Condition.Requires(oldVal).IsEqualTo(newVal);

//            })) 
//        { 
//        }
//    }

//    public class ValueOfTest<T> : TestOf<IValueOf<T>>
//    {
//        public ValueOfTest()
//            : base(LogicOf<IValueOf<T>>.New((x) =>
//            {
//                var flagDate = DateTime.Now.AddSeconds(5);
//                ICondition cond = StrategizedCondition.New(() => { return DateTime.Now < flagDate; });
//                ICondition expCond = StrategizedCondition.New(() => { return DateTime.Now > flagDate.AddSeconds(1); });

//                var newX = x.WaitUntil(cond, expCond);
//                var oldVal = x.GetValue().GraphSerializeWithDefaults();
//                var newVal = newX.GetValue().GraphSerializeWithDefaults();
//                Condition.Requires(oldVal).IsEqualTo(newVal);


//            }))
//        {
//        }
//    }

//    public class LogicTest : TestOf<ILogic>
//    {
//        public LogicTest()
//            : base(LogicOf<ILogic>.New((x) =>
//            {
//                var flagDate = DateTime.Now.AddSeconds(5);
//                ICondition cond = StrategizedCondition.New(() => { return DateTime.Now < flagDate; });
//                ICondition expCond = StrategizedCondition.New(() => { return DateTime.Now > flagDate.AddSeconds(1); });

//                var newX = x.WaitUntil(cond, expCond);
//                newX.Perform();

//            }))
//        {
//        }
//    }
//}
