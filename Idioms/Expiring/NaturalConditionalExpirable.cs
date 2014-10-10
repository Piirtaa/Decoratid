using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Conditional.Of;
using Decoratid.Idioms.Polyfacing;
using Decoratid.Idioms.WithValuing;
using Decoratid.Core.ValueOfing;
using System;

namespace Decoratid.Idioms.Expiring
{
    /// <summary>
    /// an immutable expiry date.  implicitly convertable to datetime
    /// </summary>
    [Serializable]
    public class NaturalConditionalExpirable : IExpirable, IPolyfacing
    {
        #region Ctor
        public NaturalConditionalExpirable(ICondition expiry)
        {
            this.Expiry = expiry;
        }
        #endregion

        #region IExpirable
        public virtual bool IsExpired()
        {
            return this.Expiry.Evaluate().GetValueOrDefault();
        }
        #endregion

        #region IPolyfacing
        Polyface IPolyfacing.RootFace { get; set; }
        #endregion

        #region Properties
        public ICondition Expiry { get; protected set; }
        #endregion

        #region Fluent Static
        public static NaturalConditionalExpirable New(ICondition cond)
        {
            return new NaturalConditionalExpirable(cond);
        }
        #endregion
    }

    public static class NaturalConditionalExpirableExtensions
    {
        public static Polyface IsConditionalExpirable(this Polyface root, IConditionOf<Polyface> expiry)
        {
            Condition.Requires(root).IsNotNull();
            var composited = new NaturalConditionalExpirable(expiry.WithValue(root.AsNaturalValue()));
            root.Is(composited);
            return root;
        }
        public static Polyface IsConditionalExpirable(this Polyface root, ICondition expiry)
        {
            Condition.Requires(root).IsNotNull();
            var composited = new NaturalConditionalExpirable(expiry);
            root.Is(composited);
            return root;
        }
        public static NaturalConditionalExpirable AsConditionalExpirable(this Polyface root)
        {
            Condition.Requires(root).IsNotNull();
            var rv = root.As<NaturalConditionalExpirable>();
            return rv;
        }
    }
}
