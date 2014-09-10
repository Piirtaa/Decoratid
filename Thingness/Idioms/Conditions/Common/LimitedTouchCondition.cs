using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness;
using Decoratid.Thingness.Idioms.ValuesOf;
using Decoratid.Extensions;

namespace Decoratid.Thingness.Idioms.Conditions.Common
{
    /// <summary>
    /// State holder of number of touches something has had
    /// </summary>
    /// 
    [Serializable]
    public class LimitedTouchInfo : ICanTouch
    {
        public LimitedTouchInfo(int limit)
        {
            this.TouchCount = 0;
            this.TouchLimit = limit;
            this.LastTouchedDate = DateTime.UtcNow;
        }

        public DateTime LastTouchedDate { get; protected set; }
        public int TouchCount { get; protected set; }
        public int TouchLimit { get; protected set; }

        public void Touch()
        {
            lock (this)
            {
                this.TouchCount = this.TouchCount + 1;
                this.LastTouchedDate = DateTime.UtcNow;
            }
        }
        public bool IsWithinLimit()
        {
            return this.TouchCount <= this.TouchLimit;
        }
    }

    /// <summary>
    /// mutable condition that can be mutated(eg. touched) a finite number of times.  returns true when the limit has been 
    /// reached
    /// </summary>
    /// 
    [Serializable]
    public class LimitedTouchCondition : MutableContextualCondition<LimitedTouchInfo>
    {
        #region Ctor
        public LimitedTouchCondition(int limit)
            : this(new LimitedTouchInfo(limit))
        {

        }

        /// <summary>
        /// provides factory of limit
        /// </summary>
        /// <param name="limit"></param>
        public LimitedTouchCondition(LimitedTouchInfo context)
            : base(context.ValueOf(),
             StrategizedConditionOf<LimitedTouchInfo>.New(
            (info) =>
            {
                var rv= !info.IsWithinLimit();
                return rv;
            }),
            (info) =>
            {
                info.Touch();
                return info;
            })
        {
        }
        #endregion

        #region Static Methods
        public static LimitedTouchCondition New(int limit)
        {
            return new LimitedTouchCondition(limit);
        }
        #endregion
    }
}

