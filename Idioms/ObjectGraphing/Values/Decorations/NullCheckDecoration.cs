using CuttingEdge.Conditions;
using Decoratid.Idioms.Decorating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.Stringing.Decorations;

namespace Decoratid.Idioms.ObjectGraph.Values.Decorations
{
    /// <summary>
    /// injects null check validation
    /// </summary>
    [Serializable]
    public class NullCheckDecoration : DecoratedValueManagerBase
    {
        #region Ctor
        public NullCheckDecoration(INodeValueManager decorated)
            : base(decorated)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// decorate so that if an object is null, we can't handle it by default, otherwise ask the decorated value manager
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="uow"></param>
        /// <returns></returns>
        public override bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            return base.Decorated.CanHandle(obj, uow);
        }
        public override string DehydrateValue(object obj, IGraph uow)
        {
            if (obj == null)
                return null;

            return base.Decorated.DehydrateValue(obj, uow);
        }
        public override object HydrateValue(string nodeText, IGraph uow)
        {
            if (string.IsNullOrEmpty(nodeText))
                return null;

            return base.Decorated.HydrateValue(nodeText, uow);
        }
        public override IDecorationOf<INodeValueManager> ApplyThisDecorationTo(INodeValueManager thing)
        {
            return new NullCheckDecoration(thing);
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return string.Empty;
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {
        }
        #endregion
    }




    public static class NullExtensions
    {
        #region Error Catching
        public static NullCheckDecoration DecorateWithNullCheck(this INodeValueManager mgr)
        {
            Condition.Requires(mgr).IsNotNull();
            if (mgr is NullCheckDecoration)
            {
                return (NullCheckDecoration)mgr;
            }
            return new NullCheckDecoration(mgr);
        }

        #endregion

    }
}
