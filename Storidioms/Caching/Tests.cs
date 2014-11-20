using Decoratid.Core.Conditional;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Core.ValueOfing;
using Decoratid.Extensions;
using Decoratid.Idioms.Expiring;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Decoratid.Storidioms.Evicting;

namespace Decoratid.Storidioms.Caching
{
    public class Test : TestOf<IStore>
    {
        public Test()
            : base(LogicOf<IStore>.New((x) =>
            {
                x.Clear();
                var thing = AsId<string>.New("asId1");
                var soid = thing.GetStoredObjectId();

                //build cache that polls every 5 seconds and always expires whatever is in it 
                var store = x.Caching(NamedNaturalInMemoryStore.New("caching store").Evicting(NamedNaturalInMemoryStore.New("eviction condition store"), LogicOfTo<IHasId, IExpirable>.New((o) =>
                {
                    return NaturalTrueExpirable.New();//.DecorateWithDateExpirable(DateTime.Now.AddSeconds(5000));
                }), 1000));
                var isEvicted = false;
                store.CachingStore.ItemEvicted += delegate(object sender, EventArgOf<Tuple<IHasId, IExpirable>> e)
                {
                    isEvicted = true;
                };
                //save 
                store.SaveItem(thing);
                Thread.Sleep(6000);
                //now pull from the store, itself (not the caching store) and it will repopulate the cache
                var item = store.Get<AsId<string>>("asId1");
                Assert.True(item != null);

                //explicitly check the cache
                item = store.CachingStore.Get<AsId<string>>("asId1");
                Assert.True(item != null);

                //wait 5 seconds, and check cache again
                Thread.Sleep(6000);
                item = store.CachingStore.Get<AsId<string>>("asId1");
                Assert.True(item == null);

                //now pull from the store, itself (not the caching store) and it will repopulate the cache
                item = store.Get<AsId<string>>("asId1");
                Assert.True(item != null);

                item = store.CachingStore.Get<AsId<string>>("asId1");
                Assert.True(item != null);

                //cleanup
                store.Dispose();
            }))
        {
        }
    }
}
