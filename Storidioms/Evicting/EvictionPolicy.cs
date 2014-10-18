using Decoratid.Core.Conditional;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Idioms.Counting;
using Decoratid.Idioms.Expiring;
using Decoratid.Idioms.Polyfacing;
using Decoratid.Idioms.Touching;
using System;

namespace Decoratid.Storidioms.Evicting
{
    /// <summary>
    /// static helper class that brokers different kinds of eviction conditions
    /// </summary>
    public static class EvictionPolicy
    {
        public static IExpirable BuildImmutableExpirable(DateTime expiry)
        {
            var expire = NaturalFalseExpirable.New().DecorateWithDateExpirable(expiry);
            return expire;

        }
        public static IExpirable BuildFloatingExpirable(DateTime expiry, int touchIncrementSecs)
        {
            var expire = NaturalFalseExpirable.New().DecorateWithDateExpirable(expiry).DecorateWithFloatingDateExpirable(touchIncrementSecs);
            return expire;

        }
        public static IExpirable BuildNeverExpirable()
        {
            var expire = NaturalFalseExpirable.New();
            return expire;

        }
        public static IExpirable BuildWithinWindowExpirable(DateTime startDate, DateTime endDate)
        {
            var expire = NaturalFalseExpirable.New().DecorateWithWindowExpirable(startDate, endDate);
            return expire;

        }
        public static IExpirable BuildWithinFloatingWindowExpirable(DateTime startDate, DateTime endDate, int touchIncrementSecs)
        {

            var expire = NaturalFalseExpirable.New().DecorateWithWindowExpirable(startDate, endDate).DecorateWithFloatingWindowExpirable(touchIncrementSecs);
            return expire;

        }
        public static IExpirable BuildTouchLimitExpirable(int limit)
        {
            //fluently build something that Counts, and has a condition on that count, and is touchable 
            var pf = Polyface.New();
            pf.IsCounter();
            pf.IsConditionalExpirable(StrategizedConditionOf<Polyface>.New((pf1) => { return pf1.AsCounter().Current > limit; }));
            pf.IsStrategizedTouchable(LogicOf<Polyface>.New((pf1) => { pf1.AsCounter().Increment(); }));

            //var expire = NaturalFalseExpirable.New().ExpiresWhen( .InWindow(startDate, endDate);
            // return expire;
            return pf.AsConditionalExpirable();
        }
    }
}
