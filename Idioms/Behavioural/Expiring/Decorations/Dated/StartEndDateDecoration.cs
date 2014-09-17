using CuttingEdge.Conditions;
using Decoratid.Idioms.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.Stringing.Products;

namespace Decoratid.Idioms.Expiring.Decorations.Dated
{
    public interface IHasStartAndEndDate
    {
        DateTime? StartDate { get; }
        DateTime? EndDate { get; }
    }

    /// <summary>
    /// decoration which provides a start and end date
    /// </summary>
    public class StartEndDateDecoration : WindowableDecorationBase, IHasStartAndEndDate
    {
        #region Ctor
        public StartEndDateDecoration(IWindowable decorated, DateTime? startDate, DateTime? endDate)
            : base(decorated)
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
        }
        #endregion

        #region Properties
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        #endregion

        #region Overrides
        public override bool IsInWindow(DateTime dt)
        {
            //if there's no start date, then the window doesn't exist yet and we return false
            if (this.StartDate.HasValue == false)
                return false;

            //if we're before the start date then false
            if (this.StartDate.GetValueOrDefault().ToUniversalTime() >= dt.ToUniversalTime())
                return false;

            //so now we're in the window

            //if we have no end date, then return true
            if (this.EndDate.HasValue == false)
                return true;

            if (this.EndDate.GetValueOrDefault().ToUniversalTime() >= dt.ToUniversalTime())
                return true;

            return false;
        }
        public override IDecorationOf<IWindowable> ApplyThisDecorationTo(IWindowable thing)
        {
            return new StartEndDateDecoration(thing, this.StartDate, this.EndDate);
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return LengthEncoder.LengthEncodeList(this.StartDate.ToStringUnixTime(), this.EndDate.ToStringUnixTime());
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {
            var list = LengthEncoder.LengthDecodeList(text);
            Condition.Requires(list).HasLength(2);
            this.StartDate = list[0].FromStringUnixTime();
            this.EndDate = list[1].FromStringUnixTime();
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
