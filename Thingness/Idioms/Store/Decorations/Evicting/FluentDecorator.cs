using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Thingness.Idioms.Conditions.Common;
using Decoratid.Thingness.Idioms.Store.Decorations.Evicting;
using Decoratid.Extensions;
using Decoratid.Thingness.Idioms.Store.Decorations;
using Decoratid.Thingness.Idioms.Logics;

namespace Decoratid.Thingness.Idioms.Store
{
    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Thingness.Idioms.Store namespace.
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
