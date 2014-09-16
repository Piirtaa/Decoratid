using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional.Common;
using Decoratid.Idioms.Core.ValueOfing;

namespace Decoratid.Idioms.Core.Conditional
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
