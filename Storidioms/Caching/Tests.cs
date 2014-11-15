using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Storidioms.Caching
{
    public class Test : TestOf<IStore>
    {
        public Test()
            : base(LogicOf<IStore>.New((x) =>
            {
                var thing4 = AsId<string>.New("asId1");
                var store = NaturalInMemoryStore.New().DecorateWithLocalCaching(5000);

                //save 
                store.SaveItem(thing4);

                //now pull from the store, itself (not the caching store) and it will repopulate the cache
                var item = store.Get<AsId<string>>("asId1");
                Assert.True(item != null);

                //explicitly check the cache
                item = store.CachingStore.Get<AsId<string>>("asId1");
                Assert.True(item != null);

                //wait 5 seconds, and check cache again
                Thread.Sleep(5000);
                item = store.CachingStore.Get<AsId<string>>("asId1");
                Assert.True(item == null);

                //now pull from the store, itself (not the caching store) and it will repopulate the cache
                item = store.Get<AsId<string>>("asId1");
                Assert.True(item != null);

                item = store.CachingStore.Get<AsId<string>>("asId1");
                Assert.True(item != null);

            }))
        {
        }
    }
}
