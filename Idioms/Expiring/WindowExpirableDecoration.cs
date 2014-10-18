using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Extensions;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Expiring
{
    public interface IHasWindow : IExpirable
    {
        DateTime StartDate { get; }
        DateTime EndDate {get;}
    }
    public interface IHasSettableWindow : IHasWindow
    {
        void SetWindow(DateTime start, DateTime end);
    }

    /// <summary>
    /// implements expiry by defining a window when things aren't expired
    /// </summary>
    [Serializable]
    public class WindowExpirableDecoration : ExpirableDecorationBase, IHasWindow, IHasSettableWindow
    {
        #region Ctor
        public WindowExpirableDecoration(IExpirable decorated, DateTime startDate, DateTime endDate)
            : base(decorated)
        {
            this.StartDate = startDate.ToUniversalTime();
            this.EndDate = endDate.ToUniversalTime();
        }
        #endregion

        #region ISerializable
        protected WindowExpirableDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.StartDate = info.GetDateTime("StartDate");
            this.EndDate = info.GetDateTime("EndDate");
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("StartDate", this.StartDate);
            info.AddValue("EndDate", this.EndDate);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasWindow
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        #endregion

        #region IHasSettableExpiryDate
        public void SetWindow(DateTime start, DateTime end)
        {
            this.StartDate = start;
            this.EndDate = end;
        }
        #endregion

        #region Overrides
        public override bool IsExpired()
        {
            DateTime now = DateTime.UtcNow;
            var isInWindow =  now <= this.EndDate && now >= this.StartDate;
            return !isInWindow;
        }
        public override IDecorationOf<IExpirable> ApplyThisDecorationTo(IExpirable thing)
        {
            return new WindowExpirableDecoration(thing, this.StartDate, this.EndDate);
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

    public static class WindowExpirableDecorationExtensions
    {
        public static WindowExpirableDecoration DecorateWithWindowExpirable(this IExpirable thing, DateTime startDate, DateTime endDate)
        {
            Condition.Requires(thing).IsNotNull();
            return new WindowExpirableDecoration(thing, startDate, endDate);
        }

        public static IHasExpirable InWindow(this IHasExpirable thing, DateTime startDate, DateTime endDate)
        {
            Condition.Requires(thing).IsNotNull();
            thing.Expirable = thing.Expirable.DecorateWithWindowExpirable(startDate, endDate);

            return thing;
        }
    }
}
