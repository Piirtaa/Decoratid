using CuttingEdge.Conditions;
using Decoratid.Idioms.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Idioms.Expiring.Decorations.Dated
{
    public interface IHasExpiryDate 
    {
        DateTime ExpiryDate { get; }
    }
    /// <summary>
    /// decoration which provides an expiry date
    /// </summary>
    public class ExpiryDateDecoration : ExpirableDecorationBase, IHasExpiryDate
    {
        #region Ctor
        public ExpiryDateDecoration(IExpirable decorated, DateTime expiryDate)
            : base(decorated)
        {
            this.ExpiryDate = expiryDate.ToUniversalTime();
        }
        #endregion

        #region Properties
        public DateTime ExpiryDate { get; private set; }
        #endregion

        #region Overrides
        public override bool IsExpired()
        {
            return this.ExpiryDate < DateTime.UtcNow;
        }
        public override IDecorationOf<IExpirable> ApplyThisDecorationTo(IExpirable thing)
        {
            return new ExpiryDateDecoration(thing, this.ExpiryDate);
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return this.ExpiryDate.ToUnixTime().ToString();
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {
            var unixTime = long.Parse(text);
            this.ExpiryDate = unixTime.FromUnixTime();
        }
        #endregion
    }

    public static class ExpiryDateDecorationExtensions
    {
        public static ExpiryDateDecoration DecorateWithExpiryDate(this IExpirable thing, DateTime expiryDate)
        {
            Condition.Requires(thing).IsNotNull();
            if (thing is ExpiryDateDecoration)
            {
                return (ExpiryDateDecoration)thing;
            }
            return new ExpiryDateDecoration(thing, expiryDate);
        }
    }
}
