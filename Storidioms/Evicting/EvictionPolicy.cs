using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Conditional;
using Decoratid.Core.Conditional.Common;
using Decoratid.Core.Conditional.Common.Expiry;
using Decoratid.Thingness;

namespace Decoratid.Storidioms.Evicting
{
    /// <summary>
    /// static helper class that brokers different kinds of eviction conditions
    /// </summary>
    public static class EvictionPolicy
    {
        public static Func<IHasId, ICondition> BuildImmutableExpiryCondition(DateTime expiry)
        {
            return (ihasid) =>
            {
                return new ImmutableExpiryCondition(expiry);
            };
        }
        public static Func<IHasId, ICondition> BuildFloatingExpiryCondition(DateTime expiry, int touchIncrementSecs)
        {
            return (ihasid) =>
            {
                return new FloatingExpiryCondition(new FloatingExpiryInfo(expiry, touchIncrementSecs));
            };
        }
        public static Func<IHasId, ICondition> BuildNeverExpiringCondition()
        {
            return (ihasid) =>
            {
                return new AlwaysFalseCondition();
            };
        }
        public static Func<IHasId, ICondition> BuildWithinWindowExpiryCondition(DateTime startDate, DateTime endDate)
        {
            return (ihasid) =>
            {
                return new ImmutableTimeWindowCondition(new ImmutableTimeWindowInfo(startDate, endDate));
            };
        }
        public static Func<IHasId, ICondition> BuildWithinFloatingWindowExpiryCondition(DateTime startDate, DateTime endDate, int touchIncrementSecs)
        {
            return (ihasid) =>
            {
                return new FloatingTimeWindowCondition(new FloatingTimeWindowInfo(startDate, endDate, touchIncrementSecs));
            };
        }
        public static Func<IHasId, ICondition> BuildTouchLimitExpiryCondition(int limit)
        {
            return (ihasid) =>
            {
                return new LimitedTouchCondition(limit);
            };
        }
    }
}
