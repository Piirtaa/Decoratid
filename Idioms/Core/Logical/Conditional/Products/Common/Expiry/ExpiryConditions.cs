using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.ValuesOf;

namespace Decoratid.Idioms.Core.Conditional.Common.Expiry
{
    /// <summary>
    /// returns true when expired
    /// </summary>
    /// 

    public class ImmutableExpiryCondition : ContextualCondition<ImmutableExpiryInfo>
    {
        public ImmutableExpiryCondition(ImmutableExpiryInfo context) :
            base(
            context.ValueOf(),
            StrategizedConditionOf<ImmutableExpiryInfo>.New(
            LogicOfTo<ImmutableExpiryInfo, bool?>.New(
            (info) => { return info.IsExpired(); })))
        {

        }

        public static ImmutableExpiryCondition New(DateTime expiry)
        {
            return new ImmutableExpiryCondition(new ImmutableExpiryInfo(expiry));
        }
    }
    /// <summary>
    /// returns true when expired
    /// </summary>
    public class FloatingExpiryCondition : MutableContextualCondition<FloatingExpiryInfo>
    {
        public FloatingExpiryCondition(FloatingExpiryInfo context)
            : base(context.ValueOf(),
                StrategizedConditionOf<FloatingExpiryInfo>.New((info) => { return info.IsExpired(); }),
                (info) => { info.Touch(); return info; })
        {

        }

        public static FloatingExpiryCondition New(DateTime expiry, int touchIncrementSecs)
        {
            return new FloatingExpiryCondition(new FloatingExpiryInfo(expiry, touchIncrementSecs));
        }
        public static FloatingExpiryCondition New(FloatingExpiryInfo context)
        {
            return new FloatingExpiryCondition(context);
        }
    }
}
