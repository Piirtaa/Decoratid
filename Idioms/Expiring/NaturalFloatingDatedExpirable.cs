using CuttingEdge.Conditions;
using Decoratid.Idioms.Polyfacing;
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
    [Serializable]
    public class NaturalFloatingDatedExpirable : NaturalDatedExpirable, ITouchable
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
    public static class NaturalFloatingDatedExpirableExtensions
    {
        public static Polyface IsFloatingDatedExpirable(this Polyface root, DateTime expiry,int touchIncrementSecs)
        {
            Condition.Requires(root).IsNotNull();
            var composited = new NaturalFloatingDatedExpirable(expiry, touchIncrementSecs);
            root.Is(composited);
            return root;
        }
        public static NaturalFloatingDatedExpirable AsFloatingDatedExpirable(this Polyface root)
        {
            Condition.Requires(root).IsNotNull();
            var rv = root.As<NaturalFloatingDatedExpirable>();
            return rv;
        }
    }
}
