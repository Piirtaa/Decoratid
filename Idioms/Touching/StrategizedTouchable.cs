using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Polyfacing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Touching
{
    /// <summary>
    /// an immutable expiry date.  implicitly convertable to datetime
    /// </summary>
    [Serializable]
    public class StrategizedTouchable : ITouchable, IPolyfacing
    {
        #region Ctor
        public StrategizedTouchable(ILogic logic)
        {
            Condition.Requires(logic).IsNotNull();
            this.TouchLogic = logic;
        }
        #endregion

        #region ITouchable
        public virtual void Touch()
        {
            this.TouchLogic.Perform();
        }
        #endregion

        #region IPolyfacing
        Polyface IPolyfacing.RootFace { get; set; }
        #endregion

        #region Properties
        public ILogic TouchLogic { get; protected set; }
        #endregion

        #region Fluent Static
        public static StrategizedTouchable New(ILogic logic)
        {
            return new StrategizedTouchable(logic);
        }
        #endregion
    }

    public static class StrategizedTouchableExtensions
    {

        public static Polyface IsStrategizedTouchable(this Polyface root, ILogicOf<Polyface> logic)
        {
            Condition.Requires(root).IsNotNull();
            logic.Context = root.AsNaturalValue();
            var composited = new StrategizedTouchable(logic);
            root.IsHasTouchable(composited);
            return root;
        }
        public static Polyface IsStrategizedTouchable(this Polyface root, ILogic logic)
        {
            Condition.Requires(root).IsNotNull();
            var composited = new StrategizedTouchable(logic);
            root.IsHasTouchable(composited);
            return root;
        }
        public static StrategizedTouchable AsStrategizedTouchable(this Polyface root)
        {
            Condition.Requires(root).IsNotNull();
            var rv = root.AsHasTouchable();
            
            return rv.With(x=>x.Touchable) as StrategizedTouchable;
        }



    }
}
