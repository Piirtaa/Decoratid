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
    public interface IHasFloatingExpiryDate : IHasExpiryDate, ITouchable
    {
        int TouchIncrementSecs { get; }

    }

    public class FloatingDateExpirableDecoration : ExpirableDecorationBase, IHasFloatingExpiryDate
    {
        #region Ctor
        public FloatingDateExpirableDecoration(DateExpirableDecoration decorated, int touchIncrementSecs)
            : base(decorated)
        {
            Condition.Requires(touchIncrementSecs).IsGreaterThan(0);
            this.TouchIncrementSecs = touchIncrementSecs;
        }
        #endregion

        #region IHasFloatingExpiryDate
        public DateTime ExpiryDate
        {
            get { return ((DateExpirableDecoration)this.Decorated).ExpiryDate; }
        }

        public void Touch()
        {
            DateExpirableDecoration dec = this.Decorated as DateExpirableDecoration;
            dec.SetExpiryDate(dec.ExpiryDate.AddSeconds(this.TouchIncrementSecs));
        }
        #endregion

        #region Properties
        public int TouchIncrementSecs { get; private set; }
        #endregion


        #region Overrides
        public override IDecorationOf<IExpirable> ApplyThisDecorationTo(IExpirable thing)
        {
            Condition.Requires(thing).IsOfType(typeof(DateExpirableDecoration));
            return new FloatingDateExpirableDecoration((DateExpirableDecoration)thing, this.TouchIncrementSecs);
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

    public static class FloatingDateExpirableDecorationExtensions
    {
        public static FloatingDateExpirableDecoration Float(this DateExpirableDecoration thing, int touchIncrementSecs)
        {
            Condition.Requires(thing).IsNotNull();
            return new FloatingDateExpirableDecoration(thing, touchIncrementSecs);
        }
    }
}
