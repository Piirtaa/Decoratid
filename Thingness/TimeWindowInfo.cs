using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

namespace Decoratid.Thingness
{
    /// <summary>
    /// an immutable time window
    /// </summary>
    public class ImmutableTimeWindowInfo
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="startDate">converts to UTC</param>
        /// <param name="endDate">converts to UTC</param>
        public ImmutableTimeWindowInfo(DateTime startDate, DateTime endDate)
        {
            Condition.Requires(startDate).IsLessThan(endDate);

            this.StartDate = startDate.ToUniversalTime();
            this.EndDate = endDate.ToUniversalTime();
        }
        public DateTime StartDate { get; protected set; }
        public DateTime EndDate { get; protected set; }

        /// <summary>
        /// is the date in the window.  converts the date to UTC
        /// </summary>
        /// <param name="dateTime">converts to UTC</param>
        /// <returns></returns>
        public bool IsInWindow(DateTime dateTime)
        {
            DateTime scrubbedDateTime = dateTime.ToUniversalTime();
            return scrubbedDateTime <= this.EndDate && scrubbedDateTime >= this.StartDate;
        }
    }

    /// <summary>
    /// a floating time window
    /// </summary>
    public class FloatingTimeWindowInfo : ImmutableTimeWindowInfo, ICanTouch
    {
        public FloatingTimeWindowInfo(DateTime startDate, DateTime endDate, int touchIncrementSecs)
            : base(startDate, endDate)
        {
            this.LastTouchedDate = DateTime.UtcNow;
            this.TouchIncrementSecs = touchIncrementSecs;
        }

        public DateTime LastTouchedDate { get; protected set; }
        public int TouchIncrementSecs { get; protected set; }

        public void Touch()
        {
            this.LastTouchedDate = DateTime.UtcNow;

            this.EndDate = this.EndDate.AddSeconds(this.TouchIncrementSecs);
        }
    }

}
