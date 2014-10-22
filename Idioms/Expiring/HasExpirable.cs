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
    /// wraps an expirable
    /// </summary>
    [Serializable]
    public sealed class HasExpirable : IHasExpirable, IPolyfacing
    {
        #region Ctor
        public HasExpirable(IExpirable expirable)
        {
            Condition.Requires(expirable).IsNotNull();
            this.Expirable = expirable;
        }
        #endregion

        #region IPolyfacing
        Polyface IPolyfacing.RootFace { get; set; }
        #endregion

        #region IHasExpirable
        public IExpirable Expirable { get; set; }
        #endregion

        #region IExpirable
        public bool IsExpired()
        {
            return this.Expirable.IsExpired();
        }
        #endregion
    }

    public static class HasExpirableExtensions
    {
        public static Polyface IsHasExpirable(this Polyface root, IExpirable expirable)
        {
            Condition.Requires(root).IsNotNull();
            var composited = new HasExpirable(expirable);
            root.Is(composited);
            return root;
        }
        public static HasExpirable AsHasExpirable(this Polyface root)
        {
            Condition.Requires(root).IsNotNull();
            var rv = root.As<HasExpirable>();
            return rv;
        }
    }
}
