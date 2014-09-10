using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness;
using Decoratid.Thingness.Idioms.ValuesOf;

namespace Decoratid.Thingness.Idioms.Conditions.Common.Expiry
{
    /// <summary>
    /// returns true when "In Window"
    /// </summary>
    public class ImmutableTimeWindowCondition : ContextualCondition<ImmutableTimeWindowInfo>
    {
        public ImmutableTimeWindowCondition(ImmutableTimeWindowInfo context)
            : base(context.ValueOf(),
                    new StrategizedConditionOf<ImmutableTimeWindowInfo>((info) => { return info.IsInWindow(DateTime.UtcNow); })
                    )
        {
        }

        public static ImmutableTimeWindowCondition New(DateTime startDate, DateTime endDate)
        {
            return new ImmutableTimeWindowCondition(new ImmutableTimeWindowInfo(startDate, endDate));
        }
    }
    /// <summary>
    /// returns true when "In Window"
    /// </summary>
    public class FloatingTimeWindowCondition : MutableContextualCondition<FloatingTimeWindowInfo>
    {
        public FloatingTimeWindowCondition(FloatingTimeWindowInfo context)
            : base(context.ValueOf(), 
                    new StrategizedConditionOf<FloatingTimeWindowInfo>((info) => { return info.IsInWindow(DateTime.UtcNow); }),
                            (info) => { info.Touch(); return info; })
        {
        }

        public static FloatingTimeWindowCondition New(DateTime startDate, DateTime endDate, int touchIncrementSecs)
        {
            return new FloatingTimeWindowCondition(new FloatingTimeWindowInfo(startDate, endDate, touchIncrementSecs));
        }


    }
}
