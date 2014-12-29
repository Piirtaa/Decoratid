using Decoratid.Core.Conditional;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Decoratid.Storidioms.Masking
{
    public class Test : TestOf<IStore>
    {
        public Test()
            : base(LogicOf<IStore>.New((x) =>
            {
                x.Clear();
                var thing = AsId<string>.New("asId1");
                var soid = thing.GetStoredObjectId();

                //mask commit
                IStore store = x.NoCommit();

                Assert.Throws<InvalidOperationException>(() =>
                {
                    store.SaveItem(thing);
                });

                //mask get
                store = x.NoGet();
                store.SaveItem(thing);

                Assert.Throws<InvalidOperationException>(() =>
                {
                    store.Get<AsId<string>>("asId1");
                });

                //mask search
                store = x.NoSearch();
                store.SaveItem(thing);
                var itemCopy = store.Get<AsId<string>>("asId1");

                Assert.Throws<InvalidOperationException>(() =>
                {
                    var list = store.SearchOf<AsId<string>>(LogicOfTo<AsId<string>, bool>.New((o) => { return o.Id.Equals("asId1"); }));
                });

                //mask getall
                store = x.NoGetAll();
                store.SaveItem(thing);
                itemCopy = store.Get<AsId<string>>("asId1");

                Assert.Throws<InvalidOperationException>(() =>
                {
                    var list = store.GetAll();
                });

                //mask all of them
                store = x.NoGetAll().NoCommit().NoGet().NoSearch();

                Assert.Throws<InvalidOperationException>(() =>
                {
                    store.SaveItem(thing);
                });
                Assert.Throws<InvalidOperationException>(() =>
                {
                    store.Get<AsId<string>>("asId1");
                });
                Assert.Throws<InvalidOperationException>(() =>
                {
                    var list = store.SearchOf<AsId<string>>(LogicOfTo<AsId<string>, bool>.New((o) => { return o.Id.Equals("asId1"); }));
                });
                Assert.Throws<InvalidOperationException>(() =>
                {
                    var list = store.GetAll();
                });

                //cleanup
                x.DeleteItem(soid);
            }))
        {
        }
    }
}
