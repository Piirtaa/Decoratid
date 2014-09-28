using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Expiring
{
    /// <summary>
    /// floating expiry date
    /// </summary>
    /// 
    [Serializable]
    public class NaturalFloatingDatedExpirable : NaturalDatedExpirable, ICanTouch
    {
        #region Ctor
        public NaturalFloatingDatedExpirable(DateTime expiry, int touchIncrementSecs)
            : base(expiry)
        {
            this.LastTouchedDate = DateTime.UtcNow;
            this.TouchIncrementSecs = touchIncrementSecs;
        }
        #endregion

        #region Properties
        public DateTime LastTouchedDate { get; protected set; }
        public int TouchIncrementSecs { get; protected set; }
        #endregion

        #region ICanTouch
        public void Touch()
        {
            this.LastTouchedDate = DateTime.UtcNow;

            if (!this.IsExpired())
            {
                this.ExpiryDate = this.ExpiryDate.AddSeconds(this.TouchIncrementSecs);
            }
        }
        #endregion
    }

}
