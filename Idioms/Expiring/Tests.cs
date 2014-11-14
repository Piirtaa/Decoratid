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
using Decoratid.Idioms.ObjectGraphing;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Contextual;

namespace Decoratid.Idioms.Expiring
{
    public class ExpirableTest : TestOf<Nothing>
    {
        public ExpirableTest()
            : base(LogicOf<Nothing>.New((x) =>
            {
                #region Expirable
                //create expirable
                var expirable = NaturalTrueExpirable.New();

                //test date expirable
                var dateExp = expirable.DecorateWithDateExpirable(DateTime.Now.AddSeconds(5));
                Condition.Requires(dateExp.IsExpired()).IsFalse();
                Thread.Sleep(6000);
                Condition.Requires(dateExp.IsExpired()).IsTrue();

                //test floating date 
                var floatDateExp = expirable.DecorateWithDateExpirable(DateTime.Now.AddSeconds(5)).DecorateWithFloatingDateExpirable(1);
                Condition.Requires(floatDateExp.IsExpired()).IsFalse();
                floatDateExp.Touch().Touch();
                Thread.Sleep(6000);
                Condition.Requires(floatDateExp.IsExpired()).IsFalse();
                Thread.Sleep(2000);
                Condition.Requires(floatDateExp.IsExpired()).IsTrue();

                //test window date 
                var windowExp = expirable.DecorateWithWindowExpirable(DateTime.Now.AddSeconds(1), DateTime.Now.AddSeconds(5));
                Condition.Requires(windowExp.IsExpired()).IsTrue();
                Thread.Sleep(2000);
                Condition.Requires(windowExp.IsExpired()).IsFalse();
                Thread.Sleep(6000);
                Condition.Requires(windowExp.IsExpired()).IsTrue();

                //test float window date 
                var floatWindowExp = expirable.DecorateWithWindowExpirable(DateTime.Now.AddSeconds(1), DateTime.Now.AddSeconds(5)).DecorateWithFloatingWindowExpirable(1);
                Condition.Requires(floatWindowExp.IsExpired()).IsTrue();
                Thread.Sleep(2000);
                Condition.Requires(floatWindowExp.IsExpired()).IsFalse();
                Thread.Sleep(6000);
                Condition.Requires(floatWindowExp.IsExpired()).IsTrue();
                floatWindowExp.Touch().Touch().Touch().Touch(); //touch it back to not expired
                Condition.Requires(floatWindowExp.IsExpired()).IsFalse();

                //test conditional expiry
                var cond = StrategizedConditionOf<bool>.New((o) => { return o; }).AddContext(false);
                var condExp = expirable.DecorateWithConditionalExpirable(cond);
                Condition.Requires(condExp.IsExpired()).IsFalse();
                cond.Context = true;
                Condition.Requires(condExp.IsExpired()).IsTrue();
                #endregion

                #region HasA
                //test HasA 
                var hasExp = HasExpirable.New(expirable);
                hasExp.ExpiresOn(DateTime.Now.AddSeconds(5));
                Condition.Requires(hasExp.IsExpired()).IsFalse();
                Thread.Sleep(6000);
                Condition.Requires(hasExp.IsExpired()).IsTrue();
                //test floating date 
                hasExp.ExpiryFloats(1);
                hasExp.Touch().Touch();
                Condition.Requires(hasExp.IsExpired()).IsFalse();
                Thread.Sleep(2000);
                Condition.Requires(hasExp.IsExpired()).IsTrue();

                //test window date 
                hasExp.InWindow(DateTime.Now.AddSeconds(1), DateTime.Now.AddSeconds(5));
                Condition.Requires(hasExp.IsExpired()).IsTrue();
                Thread.Sleep(2000);
                Condition.Requires(hasExp.IsExpired()).IsFalse();
                Thread.Sleep(6000);
                Condition.Requires(hasExp.IsExpired()).IsTrue();

                //test float window date 
                hasExp.InWindow(DateTime.Now.AddSeconds(1), DateTime.Now.AddSeconds(5)).WindowFloats(1);
                Condition.Requires(hasExp.IsExpired()).IsTrue();
                Thread.Sleep(2000);
                Condition.Requires(hasExp.IsExpired()).IsFalse();
                Thread.Sleep(6000);
                Condition.Requires(hasExp.IsExpired()).IsTrue();
                hasExp.Touch().Touch().Touch().Touch(); //touch it back to not expired
                Condition.Requires(hasExp.IsExpired()).IsFalse();

                //test conditional expiry
                hasExp.ExpiresWhen(cond);
                cond.Context = false;
                Condition.Requires(condExp.IsExpired()).IsFalse();
                cond.Context = true;
                Condition.Requires(condExp.IsExpired()).IsTrue();
                #endregion
            }))
        {
        }
    }
    public class ConditionTest : TestOf<ICondition>
    {
        public ConditionTest()
            : base(LogicOf<ICondition>.New((x) =>
            {
                var expiry = DateTime.Now.AddSeconds(5);
                var newX = x.HasExpirable();
                newX.ExpiresWhen(StrategizedCondition.New(() => { return DateTime.Now > expiry; }));
                newX.Evaluate();
                Thread.Sleep(6000);

                var trapX = newX.Traps();
                var trapVal = trapX.Evaluate();
                Condition.Requires(trapVal).IsEqualTo(false);
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
                var newX = x.HasExpirable(); //expire this
                newX.ExpiresWhen(StrategizedCondition.New(() => { return DateTime.Now > expiry; })); //specify how it expires
                var oldVal = newX.GetValue();//eval. should work fine
                Thread.Sleep(6000);

                //we've expired so eval should kack
                var trapX = newX.Traps(); //trap that shit
                var trapVal = trapX.GetValue();
                Condition.Requires(trapVal == null).IsTrue();

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
                newX.ExpiresWhen(StrategizedCondition.New(() => { return DateTime.Now > expiry; }));
                newX.Perform();
                Thread.Sleep(6000);

                var trapX = newX.Traps();
                trapX.Perform();

            }))
        {
        }
    }
}
