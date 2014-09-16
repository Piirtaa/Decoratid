using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Expiring
{
    /// <summary>
    /// an immutable expiry date.  implicitly convertable to datetime
    /// </summary>
    public class ImmutableExpiryInfo : IExpireable
    {
        public ImmutableExpiryInfo(DateTime expiry)
        {
            this.ExpiryDate = expiry.ToUniversalTime();
            this.DateCreated = DateTime.UtcNow;
        }
        public DateTime DateCreated { get; protected set; }
        public DateTime ExpiryDate { get; protected set; }


        public static implicit operator ImmutableExpiryInfo(DateTime expiry)
        {
            return new ImmutableExpiryInfo(expiry);
        }
        public static implicit operator DateTime(ImmutableExpiryInfo expiry)
        {
            if (expiry == null) { return DateTime.MinValue; }
            return expiry.ExpiryDate;
        }
    }

    /// <summary>
    /// floating expiry date
    /// </summary>
    public class FloatingExpiryInfo : ImmutableExpiryInfo, ICanTouch
    {
        public FloatingExpiryInfo(DateTime expiry, int touchIncrementSecs)
            : base(expiry)
        {
            this.LastTouchedDate = DateTime.UtcNow;
            this.TouchIncrementSecs = touchIncrementSecs;
        }

        public DateTime LastTouchedDate { get; protected set; }
        public int TouchIncrementSecs { get; protected set; }

        public void Touch()
        {
            this.LastTouchedDate = DateTime.UtcNow;

            if (!this.IsExpired())
            {
                this.ExpiryDate = this.ExpiryDate.AddSeconds(this.TouchIncrementSecs);
            }
        }
    }


}
