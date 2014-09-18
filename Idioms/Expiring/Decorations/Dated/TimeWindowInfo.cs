using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

namespace Decoratid.Idioms.Expiring.Core
{
    /// <summary>
    /// an immutable time window
    /// </summary>
    public class TimeWindowInfo : IWindowable
    {
        #region Ctor
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
        #endregion

        #region Fluent Static
        public static ImmutableTimeWindowInfo New(DateTime startDate, DateTime endDate)
        {
            return new ImmutableTimeWindowInfo(startDate, endDate);
        }
        #endregion

        #region Properties
        public DateTime StartDate { get; protected set; }
        public DateTime EndDate { get; protected set; }
        #endregion

        #region Methods
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
        #endregion
    }

    /// <summary>
    /// a floating time window
    /// </summary>
    public class FloatingTimeWindowInfo : ImmutableTimeWindowInfo, ICanTouch
    {
        #region Ctor
        public FloatingTimeWindowInfo(DateTime startDate, DateTime endDate, int touchIncrementSecs)
            : base(startDate, endDate)
        {
            this.LastTouchedDate = DateTime.UtcNow;
            this.TouchIncrementSecs = touchIncrementSecs;
        }
        #endregion

        #region Properties
        public DateTime LastTouchedDate { get; protected set; }
        public int TouchIncrementSecs { get; protected set; }
        #endregion

        #region Methods
        public void Touch()
        {
            this.LastTouchedDate = DateTime.UtcNow;

            this.EndDate = this.EndDate.AddSeconds(this.TouchIncrementSecs);
        }
        #endregion
    }

}
