using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Thingness.Idioms.Conditions.Common.Expiry;
using Decoratid.Thingness;

namespace Decoratid.Thingness.Idioms.Store.Implementations
{
    /// <summary>
    /// Helper class that builds EvictionConditionFactories
    /// </summary>
    public static class EvictionConditionFactoryFactory
    {
        public static Func<IHasId, ICondition> BuildFloatingEvictionConditionFactory(int intSecsFromNow, int touchIncrementSecs)
        {
            return (key) =>
            {
                return new FloatingExpiryCondition( new FloatingExpiryInfo( DateTime.UtcNow.AddSeconds(intSecsFromNow), touchIncrementSecs));
            };
        }
        public static Func<IHasId, ICondition> BuildFloatingEvictionConditionFactory(Func<DateTime> expiryFactory, int touchIncrementSecs)
        {
            return (key) =>
            {
                return new FloatingExpiryCondition( new FloatingExpiryInfo(expiryFactory(), touchIncrementSecs));
            };
        }
        public static Func<IHasId, ICondition> BuildAbsoluteEvictionConditionFactory(int intSecsFromNow)
        {
            return (key) =>
            {
                return new ImmutableExpiryCondition( new ImmutableExpiryInfo(DateTime.UtcNow.AddSeconds(intSecsFromNow)));
            };
        }
        public static Func<IHasId, ICondition> BuildAbsoluteEvictionConditionFactory(Func<DateTime> expiryFactory)
        {
            return (key) =>
            {
                return new ImmutableExpiryCondition(new ImmutableExpiryInfo(expiryFactory()));
            };
        }
    }
}
