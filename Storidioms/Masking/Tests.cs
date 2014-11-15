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

namespace Decoratid.Storidioms.Masking
{
    public class Test : TestOf<IStore>
    {
        public Test()
            : base(LogicOf<IStore>.New((x) =>
            {
                var thing4 = AsId<string>.New("asId1");

                //mask commit
                var store = NaturalInMemoryStore.New().DecorateWithoutCommit();

                Assert.Throws<InvalidOperationException>(() =>
                {
                    store.SaveItem(thing4);
                });

                //mask get
                store = NaturalInMemoryStore.New().DecorateWithoutGet();
                store.SaveItem(thing4);

                Assert.Throws<InvalidOperationException>(() =>
                {
                    store.Get<AsId<string>>("asId1");
                });

                //mask search
                store = NaturalInMemoryStore.New().DecorateWithoutSearch();
                store.SaveItem(thing4);
                var itemCopy = store.Get<AsId<string>>("asId1");

                Assert.Throws<InvalidOperationException>(() =>
                {
                    var list = store.Search<AsId<string>>(SearchFilter.New((x) => { return x.Id.Equals("asId1"); }));
                });

                //mask getall
                store = NaturalInMemoryStore.New().DecorateWithoutGetAll();
                store.SaveItem(thing4);
                itemCopy = store.Get<AsId<string>>("asId1");

                Assert.Throws<InvalidOperationException>(() =>
                {
                    var list = store.GetAll();
                });

                //mask all of them
                store = NaturalInMemoryStore.New().DecorateWithoutGetAll().DecorateWithoutCommit().DecorateWithoutGet().DecorateWithoutSearch();

                Assert.Throws<InvalidOperationException>(() =>
                {
                    store.SaveItem(thing4);
                });
                Assert.Throws<InvalidOperationException>(() =>
                {
                    store.Get<AsId<string>>("asId1");
                });
                Assert.Throws<InvalidOperationException>(() =>
                {
                    var list = store.Search<AsId<string>>(SearchFilter.New((x) => { return x.Id.Equals("asId1"); }));
                });
                Assert.Throws<InvalidOperationException>(() =>
                {
                    var list = store.GetAll();
                });

            }))
        {
        }
    }
}
