using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Expiring.Decorations.Dated
{
    /// <summary>
    /// an immutable expiry date.  implicitly convertable to datetime
    /// </summary>
    public class ExpiryInfo : IExpirable
    {
        #region Ctor
        public ExpiryInfo(DateTime expiry)
        {
            this.ExpiryDate = expiry.ToUniversalTime();
        }
        #endregion

        #region Fluent Static
        public static ExpiryInfo New(DateTime expiry)
        {
            return new ExpiryInfo(expiry);
        }
        #endregion

        #region Properties
        public DateTime ExpiryDate { get; protected set; }
        #endregion

        #region Implicit Conversion Methods
        public static implicit operator ExpiryInfo(DateTime expiry)
        {
            return new ExpiryInfo(expiry);
        }
        public static implicit operator DateTime(ExpiryInfo expiry)
        {
            if (expiry == null) { return DateTime.MinValue; }
            return expiry.ExpiryDate;
        }
        #endregion
    }

    ///// <summary>
    ///// floating expiry date
    ///// </summary>
    //public class FloatingExpiryInfo : ImmutableExpiryInfo, ICanTouch
    //{
    //    public FloatingExpiryInfo(DateTime expiry, int touchIncrementSecs)
    //        : base(expiry)
    //    {
    //        this.LastTouchedDate = DateTime.UtcNow;
    //        this.TouchIncrementSecs = touchIncrementSecs;
    //    }

    //    public DateTime LastTouchedDate { get; protected set; }
    //    public int TouchIncrementSecs { get; protected set; }

    //    public void Touch()
    //    {
    //        this.LastTouchedDate = DateTime.UtcNow;

    //        if (!this.IsExpired())
    //        {
    //            this.ExpiryDate = this.ExpiryDate.AddSeconds(this.TouchIncrementSecs);
    //        }
    //    }
    //}


}
