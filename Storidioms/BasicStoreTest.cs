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
using Decoratid.Core.Identifying;
using Decoratid.Core.Contextual;
using CuttingEdge.Conditions;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using Decoratid.Extensions;
using Decoratid.Idioms.ObjectGraphing;

namespace Decoratid.Storidioms
{
    public class BasicStoreTest : TestOf<Tuple<IStore, IHasId>>
    {
        public BasicStoreTest()
            : base(LogicOf<Tuple<IStore, IHasId>>.New((tuple) =>
            {
                CompareLogic compareLogic = new CompareLogic();

                //build function to compare objects a bunch of diff ways
                Func<object, object, bool> equivTest = (thing1, thing2) =>
                {
                    //try comparelogic first
                    if (!thing1.IsDeepEqual(thing2))
                        return false;

                    //try objectgraphing compare
                    var dat1 = thing1.GraphSerializeWithDefaults();
                    var dat2 = thing2.GraphSerializeWithDefaults();

                    return (dat1.Equals(dat2));
                };


                var store = tuple.Item1;
                var item = tuple.Item2;
                var itemId = item.GetStoredObjectId();

                //save it
                store.SaveItem(item);

                //load it & compare it
                IHasId item2 = store.Get(itemId);
                Assert.NotNull(item2);
                Assert.True(equivTest(item, item2));

                //search for it & compare it
                var filter = SearchFilter.New((x) =>
                {
                    return x.GetStoredObjectId().Equals(itemId);
                });

                var items = store.Search<IHasId>(filter);
                Assert.True(items.Count == 1);

                //delete it
                store.DeleteItem(items.First().GetStoredObjectId());
                item2 = store.Get(itemId);
                Assert.True(item2 == null);

            }))
        {
        }
    }
}
