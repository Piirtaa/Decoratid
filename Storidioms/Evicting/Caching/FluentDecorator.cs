using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Conditional.Common.Expiry;
using Decoratid.Core.Storing;
using Decoratid.Storidioms.Caching;
using Decoratid.Storidioms.Evicting;
using Decoratid.Storidioms;
using Decoratid.Core.Logical;

namespace Decoratid.Core.Storing
{
    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Core.Storing namespace.
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
            var evictingStore = new NaturalInMemoryStore().DecorateWithEviction(new NaturalInMemoryStore(), defaultItemEvictionConditionFactory,  backgroundIntervalMSecs);
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
            var evictingStore = new NaturalInMemoryStore().DecorateWithEviction(new NaturalInMemoryStore(),
                LogicOfTo<IHasId,ICondition>.New((it)=>{
                return new FloatingExpiryCondition(new Thingness.FloatingExpiryInfo(DateTime.UtcNow, secondsToCache));
            }), 5000);
            return new CacheDecoration(evictingStore, decorated);
        }
    }



}
