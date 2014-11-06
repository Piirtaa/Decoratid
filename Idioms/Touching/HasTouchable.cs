using CuttingEdge.Conditions;
using Decoratid.Idioms.Polyfacing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Touching
{
    /// <summary>
    /// wraps an Touchable
    /// </summary>
    [Serializable]
    public sealed class HasTouchable : IHasTouchable, ITouchable, IPolyfacing
    {
        #region Ctor
        public HasTouchable(ITouchable Touchable)
        {
            Condition.Requires(Touchable).IsNotNull();
            this.Touchable = Touchable;
        }
        #endregion

        #region IPolyfacing
        Polyface IPolyfacing.RootFace { get; set; }
        #endregion

        #region IHasTouchable
        public ITouchable Touchable { get; private set; }
        #endregion

        #region ITouchable
        public ITouchable Touch()
        {
            this.Touchable.Touch();
            return this;
        }
        #endregion
    }

    public static class HasTouchableExtensions
    {
        public static Polyface IsHasTouchable(this Polyface root, ITouchable touchable)
        {
            Condition.Requires(root).IsNotNull();
            var composited = new HasTouchable(touchable);
            root.Is(composited);
            return root;
        }
        public static HasTouchable AsHasTouchable(this Polyface root)
        {
            Condition.Requires(root).IsNotNull();
            var rv = root.As<HasTouchable>();
            return rv;
        }
    }
}
