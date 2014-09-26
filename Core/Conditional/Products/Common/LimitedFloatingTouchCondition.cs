using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Conditions.Common
{
    /// <summary>
    /// State holder of number of touches something has had
    /// </summary>
    public class FloatingTouchCountInfo
    {
        public int TouchCount { get; set; }
        public int TouchLimit { get; set; }
    }
    /// <summary>
    /// returns true when number of touches in the time window have exceeded the provided number
    /// </summary>
    public class LimitedFloatingTouchCondition : MutableContextualStrategizedCondition<LimitedTouchCountInfo>
    {

        public LimitedFloatingTouchCondition(int limit)
            : base()
        {
            //create state
            this.Context = new LimitedTouchCountInfo() { TouchCount = limit };

            //define behaviour
            this.ConditionStrategy = (info) =>
            {
                return info.TouchCount <= info.TouchLimit;
            };

            //every time we touch the condition, count gets incremented
            this.MutateStrategy = (info) =>
            {
                lock (info)
                {
                    info.TouchCount = info.TouchCount++;
                }
            };
        }
    }
}
