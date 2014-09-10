using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Thingness.Idioms.Conditions.Common.Expiry;
using Decoratid.Thingness.Idioms.Store.CoreStores;
using Decoratid.Thingness.Idioms.Store.Decorations.Caching;
using Decoratid.Thingness.Idioms.Store.Decorations.Evicting;
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
        /// gets the caching layer
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static ICachingStore GetCachingDecoration(this IStore decorated)
        {
            return decorated.FindDecoratorOf<ICachingStore>(false);
        }

        /// <summary>
        /// adds caching to the store, with the cache supplied.  Note: this is how we might inject
        /// a distributed cache.
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="cachingStore"></param>
        /// <returns></returns>
        public static CacheDecoration DecorateWithCaching(this IStore decorated, IEvictingStore cachingStore)
        {
            Condition.Requires(decorated).IsNotNull();
            return new CacheDecoration(cachingStore, decorated);
        }
        /// <summary>
        /// adds an inmemory cache with the specified policy and ticker resolution
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="defaultItemEvictionConditionFactory"></param>
        /// <param name="backgroundIntervalMSecs"></param>
        /// <returns></returns>
        public static CacheDecoration DecorateWithLocalCaching(this IStore decorated,
            LogicOfTo<IHasId, ICondition> defaultItemEvictionConditionFactory, 
            double backgroundIntervalMSecs = 30000)
        {
            Condition.Requires(decorated).IsNotNull();

            //build the evicting store - notice the very fucking fluent way it works.  ya.  
            var evictingStore = new InMemoryStore().DecorateWithEviction(new InMemoryStore(), defaultItemEvictionConditionFactory,  backgroundIntervalMSecs);
            return new CacheDecoration(evictingStore, decorated);
        }
        /// <summary>
        /// adds an inmemory cache with a floating expiry policy
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="secondsToCache"></param>
        /// <returns></returns>
        public static CacheDecoration DecorateWithLocalCaching(this IStore decorated, int secondsToCache)
        {
            Condition.Requires(decorated).IsNotNull();
            var evictingStore = new InMemoryStore().DecorateWithEviction(new InMemoryStore(),
                LogicOfTo<IHasId,ICondition>.New((it)=>{
                return new FloatingExpiryCondition(new Thingness.FloatingExpiryInfo(DateTime.UtcNow, secondsToCache));
            }), 5000);
            return new CacheDecoration(evictingStore, decorated);
        }
    }



}
