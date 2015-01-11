using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Idioms.Expiring;
using Decoratid.Storidioms.Evicting;
using System;
using System.Linq;

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
    public class CacheDecoration : DecoratedStoreBase, ICachingStore
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
        public CacheDecoration(IStore decorated, IEvictingStore cachingStore)
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
            var returnValue = new CacheDecoration(store, this.CachingStore);
            return returnValue;
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
            //update the cache
            this.CachingStore.Commit(bag);

            //do reg commit
            this.Decorated.Commit(bag);

        }
        #endregion
    }

    public static class CacheDecorationExtensions
    {
        /// <summary>
        /// gets the caching layer
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static ICachingStore GetCache(this IStore decorated)
        {
            return decorated.As<ICachingStore>(false);
        }

        /// <summary>
        /// adds caching to the store, with the cache supplied.  Note: this is how we might inject
        /// a distributed cache.
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="cachingStore"></param>
        /// <returns></returns>
        public static CacheDecoration Caching(this IStore decorated, IEvictingStore cachingStore)
        {
            Condition.Requires(decorated).IsNotNull();
            return new CacheDecoration(decorated, cachingStore);
        }
    }

}
