using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Extensions;
using Decoratid.Storidioms.Evicting;
using Decoratid.Thingness;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Storidioms.Caching
{
    /// <summary>
    /// defines a store that retrieves items first from a "CachingStore" (which itself evicts items according to
    /// the DefaultItemEvictionConditionFactory) on a Get.  If the cache doesn't have it, it reloads from the underlying 
    /// store.
    /// </summary>
    public interface ICachingStore : IDecoratedStore
    {
        IEvictingStore CachingStore { get; set; }
    }

    /// <summary>
    /// keeps a local cache and only asks the decorated registry for things when it doesn't have them
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class CacheDecoration : DecoratedStoreBase, ICachingStore, IHasHydrationMap
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        /// <summary>
        /// ctor with no default eviction condition factory.  any items added will not be evicted without an eviction being set
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="backgroundIntervalMSecs"></param>
        public CacheDecoration(IEvictingStore cachingStore,
            IStore decorated)
            : base(decorated)
        {
            Condition.Requires(cachingStore).IsNotNull();
            this.CachingStore = cachingStore;
        }
        #endregion

        #region ICacheDecoration
        public IEvictingStore CachingStore { get; set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new CacheDecoration(this.CachingStore, store);
            return returnValue;
        }
        #endregion

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var hydrationMap = new HydrationMapValueManager<CacheDecoration>();
            hydrationMap.RegisterDefault("CachingStore", x => x.CachingStore, (x, y) => { x.CachingStore = y as IEvictingStore; });
            return hydrationMap;
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return this.GetHydrationMap().DehydrateValue(this, uow);
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {
            this.GetHydrationMap().HydrateValue(this, text, uow);
        }
        #endregion

        #region Overrides
        public override IHasId Get(IStoredObjectId soId)
        {
            var retval = this.CachingStore.Get(soId);

            if (retval == null)
            {
                //hit the actual store
                lock (this._stateLock)
                {
                    retval = this.Decorated.Get(soId);

                    //something is here, save it in the cache
                    if (retval != null)
                    {
                        this.CachingStore.SaveItem(retval);
                    }
                }
            }

            return retval;
        }
        public override void Commit(ICommitBag bag)
        {
            //for delete items, clear cache
            var delItems = bag.ItemsToDelete.ToList();
            var cacheCommitBag = CommitBag.New();
            delItems.WithEach(id =>
            {
                cacheCommitBag.ItemsToDelete.Add(id);
            });
            this.CachingStore.Commit(cacheCommitBag);

            //do reg commit
            base.Commit(bag);

        }
        #endregion
    }
}
