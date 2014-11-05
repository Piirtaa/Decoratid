using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Idioms.Touching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Expiring
{
    public interface IHasFloatingWindow : IHasWindow, ITouchable
    {
        int TouchIncrementSecs { get; }

    }

    public class FloatingWindowExpirableDecoration : ExpirableDecorationBase, IHasFloatingWindow
    {
        #region Ctor
        public FloatingWindowExpirableDecoration(WindowExpirableDecoration decorated, int touchIncrementSecs)
            : base(decorated)
        {
            Condition.Requires(touchIncrementSecs).IsGreaterThan(0);
            this.TouchIncrementSecs = touchIncrementSecs;
        }
        #endregion

        #region IHasFloatingExpiryDate
        public DateTime StartDate
        {
            get { return ((WindowExpirableDecoration)this.Decorated).StartDate; }
        }
        public DateTime EndDate
        {
            get { return ((WindowExpirableDecoration)this.Decorated).EndDate; }
        }
        public void Touch()
        {
            WindowExpirableDecoration dec = this.Decorated as WindowExpirableDecoration;
            dec.SetWindow(dec.StartDate, dec.EndDate.AddSeconds(this.TouchIncrementSecs));
        }
        #endregion

        #region Properties
        public int TouchIncrementSecs { get; private set; }
        #endregion


        #region Overrides
        public override IDecorationOf<IExpirable> ApplyThisDecorationTo(IExpirable thing)
        {
            Condition.Requires(thing).IsOfType(typeof(WindowExpirableDecoration));
            return new FloatingWindowExpirableDecoration((WindowExpirableDecoration)thing, this.TouchIncrementSecs);
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

    public static class FloatingWindowExpirableDecorationExtensions
    {
        public static FloatingWindowExpirableDecoration DecorateWithFloatingWindowExpirable(this WindowExpirableDecoration thing, int touchIncrementSecs)
        {
            Condition.Requires(thing).IsNotNull();
            return new FloatingWindowExpirableDecoration(thing, touchIncrementSecs);
        }

        public static IHasExpirable WindowFloats(this IHasExpirable thing, int touchIncrementSecs)
        {
            Condition.Requires(thing).IsNotNull();
            if (!(thing.Expirable is WindowExpirableDecoration))
                throw new InvalidOperationException("expirable must be WindowExpirableDecoration");

            var dateExpire = thing.Expirable as WindowExpirableDecoration;
            thing.Expirable = dateExpire.DecorateWithFloatingWindowExpirable(touchIncrementSecs);

            return thing;
        }
    }
}
