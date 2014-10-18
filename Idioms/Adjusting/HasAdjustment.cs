using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using Decoratid.Idioms.Polyfacing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Adjusting
{
    /// <summary>
    /// wraps an Adjustment
    /// </summary>
    [Serializable]
    public sealed class HasAdjustment<T> : IHasAdjustment<T>, IPolyfacing
    {
        #region Ctor
        public HasAdjustment(IAdjustment<T> Adjustment)
        {
            Condition.Requires(Adjustment).IsNotNull();
            this.Adjustment = Adjustment;
        }
        #endregion

        #region IPolyfacing
        Polyface IPolyfacing.RootFace { get; set; }
        #endregion

        #region IHasAdjustment
        public IAdjustment<T> Adjustment { get; set; }
        #endregion

        #region IAdjustment
        public LogicOfTo<T, T> AdjustmentLogic { get { return this.Adjustment.AdjustmentLogic; } }
        public T AdjustedValue { get { return this.Adjustment.AdjustedValue; } }
        #endregion
    }

    public static class HasAdjustmentExtensions
    {
        public static Polyface IsHasAdjustment<T>(this Polyface root, IAdjustment<T> adjustment)
        {
            Condition.Requires(root).IsNotNull();
            var composited = new HasAdjustment<T>(adjustment);
            root.Is(composited);
            return root;
        }
        public static HasAdjustment<T> AsHasAdjustment<T>(this Polyface root)
        {
            Condition.Requires(root).IsNotNull();
            var rv = root.As<HasAdjustment<T>>();
            return rv;
        }
    }
}
