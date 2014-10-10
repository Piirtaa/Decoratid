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
        public static Func<IHasId, IExpirable> BuildImmutableExpiryCondition(DateTime expiry)
        {
            return (ihasid) =>
            {
                var expire = NaturalFalseExpirable.New().ExpiresOn(expiry);
                return expire;
            };
        }
        public static Func<IHasId, IExpirable> BuildFloatingExpiryCondition(DateTime expiry, int touchIncrementSecs)
        {
            return (ihasid) =>
            {
                var expire = NaturalFalseExpirable.New().ExpiresOn(expiry).Float(touchIncrementSecs);
                return expire;
            };
        }
        public static Func<IHasId, IExpirable> BuildNeverExpiringCondition()
        {
            return (ihasid) =>
            {
                var expire = NaturalFalseExpirable.New();
                return expire;
            };
        }
        public static Func<IHasId, IExpirable> BuildWithinWindowExpiryCondition(DateTime startDate, DateTime endDate)
        {
            return (ihasid) =>
            {
                var expire = NaturalFalseExpirable.New().InWindow(startDate, endDate);
                return expire;
            };
        }
        public static Func<IHasId, IExpirable> BuildWithinFloatingWindowExpiryCondition(DateTime startDate, DateTime endDate, int touchIncrementSecs)
        {
            return (ihasid) =>
            {
                var expire = NaturalFalseExpirable.New().InWindow(startDate, endDate).Float(touchIncrementSecs);
                return expire;
            };
        }
        public static Func<IHasId, IExpirable> BuildTouchLimitExpiryCondition(int limit)
        {
            return (ihasid) =>
            {
                //fluently build something that Counts, and has a condition on that count, and is touchable 
                var pf = Polyface.New();
                pf.IsCounter();
                pf.IsConditionalExpirable(StrategizedConditionOf<Polyface>.New((pf1) => { return pf1.AsCounter().Current > limit; }));
                pf.IsStrategizedTouchable(LogicOf<Polyface>.New((pf1) => { pf1.AsCounter().Increment(); }));

                //var expire = NaturalFalseExpirable.New().ExpiresWhen( .InWindow(startDate, endDate);
                // return expire;
                return pf.AsConditionalExpirable();
            };
        }
    }
}
