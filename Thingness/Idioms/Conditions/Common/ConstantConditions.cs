using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness;
using Decoratid.Thingness.Idioms.Logics;

namespace Decoratid.Thingness.Idioms.Conditions.Common
{
    /// <summary>
    /// Always true condition
    /// </summary>
    /// 
    [Serializable]
    public class AlwaysTrueCondition : StrategizedCondition
    {
        public AlwaysTrueCondition()
            : base(
                () =>
                {
                    return true;
                })
        {
        }
        //protected AlwaysTrueCondition(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //}

        public static AlwaysTrueCondition New()
        {
            return new AlwaysTrueCondition();
        }
    }
    /// <summary>
    /// always false condition
    /// </summary>
    /// 
    [Serializable]
    public class AlwaysFalseCondition : StrategizedCondition
    {
        public AlwaysFalseCondition()
            : base(
               () =>
               {
                   return false;
               })
        {
        }
        //protected AlwaysFalseCondition(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //}
        public static AlwaysFalseCondition New()
        {
            return new AlwaysFalseCondition();
        }
    }
}
