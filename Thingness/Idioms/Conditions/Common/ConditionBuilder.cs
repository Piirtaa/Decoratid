using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Conditions.Common;
using Decoratid.Thingness.Idioms.ValuesOf;

namespace Decoratid.Thingness.Idioms.Conditions
{
    
    public static partial class ConditionBuilder
    {
        public static AlwaysTrueCondition NewAlwaysTrueCondition()
        {
            return AlwaysTrueCondition.New();
        }

        public static AlwaysFalseCondition NewAlwaysFalseCondition()
        {
            return AlwaysFalseCondition.New();
        }

        public static LimitedTouchCondition NewLimitedTouchCondition(int limit)
        {
            return LimitedTouchCondition.New(limit);
        }
    }
}
