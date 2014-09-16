using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional.Common.Expiry;
using Decoratid.Idioms.Core.ValueOfing;

namespace Decoratid.Idioms.Core.Conditional
{
    
    public static partial class ConditionBuilder
    {
        public static ImmutableExpiryCondition NewImmutableExpiryCondition(DateTime expiry)
        {
            return ImmutableExpiryCondition.New(expiry);
        }
        public static FloatingExpiryCondition NewFloatingExpiryCondition(DateTime expiry, int touchIncrementSecs)
        {
            return FloatingExpiryCondition.New(expiry, touchIncrementSecs);
        }
        public static ImmutableTimeWindowCondition NewImmutableTimeWindowCondition(DateTime startDate, DateTime endDate)
        {
            return ImmutableTimeWindowCondition.New(startDate, endDate);
        }
        public static FloatingTimeWindowCondition NewFloatingTimeWindowCondition(DateTime startDate, DateTime endDate, int touchIncrementSecs)
        {
            return FloatingTimeWindowCondition.New(startDate, endDate, touchIncrementSecs);
        }
    }
}
