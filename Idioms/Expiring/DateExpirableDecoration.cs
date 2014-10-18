using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Extensions;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Expiring
{
    public interface IHasExpiryDate : IExpirable
    {
        DateTime ExpiryDate { get; }
    }
    public interface IHasSettableExpiryDate : IHasExpiryDate
    {
        void SetExpiryDate(DateTime dt);
    }

    /// <summary>
    /// implements expiry by expiry date
    /// </summary>
    [Serializable]
    public class DateExpirableDecoration : ExpirableDecorationBase, IHasExpiryDate, IHasSettableExpiryDate
    {
        #region Ctor
        public DateExpirableDecoration(IExpirable decorated, DateTime expiryDate)
            : base(decorated)
        {
            this.ExpiryDate = expiryDate.ToUniversalTime();
        }
        #endregion

        #region ISerializable
        protected DateExpirableDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ExpiryDate = info.GetDateTime("ExpiryDate");
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ExpiryDate", this.ExpiryDate);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasExpiryDate
        public DateTime ExpiryDate { get; private set; }
        #endregion

        #region IHasSettableExpiryDate
        public void SetExpiryDate(DateTime dt)
        {
            this.ExpiryDate = dt;
        }
        #endregion

        #region Overrides
        public override bool IsExpired()
        {
            return this.ExpiryDate < DateTime.UtcNow;
        }
        public override IDecorationOf<IExpirable> ApplyThisDecorationTo(IExpirable thing)
        {
            return new DateExpirableDecoration(thing, this.ExpiryDate);
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

    public static class DateExpirableDecorationExtensions
    {
        public static DateExpirableDecoration DecorateWithDateExpirable(this IExpirable thing, DateTime expiryDate)
        {
            Condition.Requires(thing).IsNotNull();
            return new DateExpirableDecoration(thing, expiryDate);
        }
        public static IHasExpirable On(this IHasExpirable thing, DateTime expiryDate)
        {
            Condition.Requires(thing).IsNotNull();
            thing.Expirable = thing.Expirable.DecorateWithDateExpirable(expiryDate);

            return thing;
        }

    }
}
