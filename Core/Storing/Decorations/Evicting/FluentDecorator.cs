using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Conditional.Common;
using Decoratid.Idioms.Storing.Decorations.Evicting;
using Decoratid.Extensions;
using Decoratid.Idioms.Storing.Decorations;
using Decoratid.Core.Logical;

namespace Decoratid.Idioms.Storing
{
    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Idioms.Storing namespace.
    /// </summary>
    public static partial class FluentDecorator
    {
        /// <summary>
        /// gets the evicting layer
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static EvictDecoration GetEvictingDecoration(this IStore decorated)
        {
            return decorated.FindDecoratorOf<EvictDecoration>(true);
        }

        /// <summary>
        /// adds eviction to a store. 
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="evictionConditionStore"></param>
        /// <param name="defaultItemEvictionConditionFactory"></param>
        /// <param name="backgroundIntervalMSecs"></param>
        /// <returns></returns>
        public static EvictDecoration DecorateWithEviction(this IStore decorated,
            IStore evictionConditionStore,
            LogicOfTo<IHasId, ICondition> defaultItemEvictionConditionFactory,
            double backgroundIntervalMSecs = 30000)
        {
            Condition.Requires(decorated).IsNotNull();

            return new EvictDecoration(evictionConditionStore, decorated,
                defaultItemEvictionConditionFactory, backgroundIntervalMSecs);
        }

        #region General Eviction
        public static void SaveEvictingItem(this IEvictingStore store, IHasId obj, ICondition evictingCondition)
        {
            if (store == null)
                return;

            store.Commit(new CommitBag().MarkItemSaved(obj), evictingCondition);
        }
        #endregion

        #region One Time
        public static void SaveLimitedTouchItem(this IEvictingStore store, IHasId obj, int touches)
        {
            if (store == null)
                return;

            store.Commit(new CommitBag().MarkItemSaved(obj), new LimitedTouchCondition(touches));
        }
        public static void SaveOneTimeItem(this IEvictingStore store, IHasId obj)
        {
            if (store == null)
                return;

            store.Commit(new CommitBag().MarkItemSaved(obj), new LimitedTouchCondition(1));
        }
        #endregion
    }



}
