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
    /// an immutable expiry date.  implicitly convertable to datetime
    /// </summary>
    [Serializable]
    public class NaturalDatedExpirable : IExpirable, IPolyfacing
    {
        #region Ctor
        public NaturalDatedExpirable(DateTime expiry)
        {
            this.ExpiryDate = expiry.ToUniversalTime();
            this.DateCreated = DateTime.UtcNow;
        }
        #endregion

        #region IExpirable
        public virtual bool IsExpired()
        {
            return this.ExpiryDate.ToUniversalTime() < DateTime.UtcNow;
        }
        #endregion

        #region IPolyfacing
        Polyface IPolyfacing.RootFace { get; set; }
        #endregion

        #region Properties
        public DateTime DateCreated { get; protected set; }
        public DateTime ExpiryDate { get; protected set; }
        #endregion

        #region Implicit Conversions
        public static implicit operator NaturalDatedExpirable(DateTime expiry)
        {
            return new NaturalDatedExpirable(expiry);
        }
        public static implicit operator DateTime(NaturalDatedExpirable expiry)
        {
            if (expiry == null) { return DateTime.MinValue; }
            return expiry.ExpiryDate;
        }
        #endregion

    }

    public static class NaturalDatedExpirableExtensions
    {
        public static Polyface IsDatedExpirable(this Polyface root, DateTime expiry)
        {
            Condition.Requires(root).IsNotNull();
            var composited = new NaturalDatedExpirable(expiry);
            root.Is(composited);
            return root;
        }
        public static NaturalDatedExpirable AsDatedExpirable(this Polyface root)
        {
            Condition.Requires(root).IsNotNull();
            var rv = root.As<NaturalDatedExpirable>();
            return rv;
        }
    }
}
