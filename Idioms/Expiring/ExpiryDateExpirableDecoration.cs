using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Extensions;
using System;

namespace Decoratid.Idioms.Expiring
{
    public interface IHasExpiryDate 
    {
        DateTime ExpiryDate { get; }
    }

    /// <summary>
    /// implements expiry by expiry date
    /// </summary>
    public class ExpiryDateExpirableDecoration : ExpirableDecorationBase, IHasExpiryDate
    {
        #region Ctor
        public ExpiryDateExpirableDecoration(IExpirable decorated, DateTime expiryDate)
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
            return new ExpiryDateExpirableDecoration(thing, this.ExpiryDate);
        }
        #endregion

        //#region IDecorationHydrateable
        //public override string DehydrateDecoration(IGraph uow = null)
        //{
        //    return this.ExpiryDate.ToUnixTime().ToString();
        //}
        //public override void HydrateDecoration(string text, IGraph uow = null)
        //{
        //    var unixTime = long.Parse(text);
        //    this.ExpiryDate = unixTime.FromUnixTime();
        //}
        //#endregion
    }

    public static class ExpiryDateDecorationExtensions
    {
        public static ExpiryDateExpirableDecoration UseExpiryDate(this IExpirable thing, DateTime expiryDate)
        {
            Condition.Requires(thing).IsNotNull();
            if (thing is ExpiryDateExpirableDecoration)
            {
                return (ExpiryDateExpirableDecoration)thing;
            }
            return new ExpiryDateExpirableDecoration(thing, expiryDate);
        }
    }
}
