using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using System;

namespace Decoratid.Idioms.ObjectGraphing.Values.Decorations
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
