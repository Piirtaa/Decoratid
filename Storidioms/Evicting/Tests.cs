//using Decoratid.Core.Conditional;
//using Decoratid.Core.Identifying;
//using Decoratid.Core.Logical;
//using Decoratid.Core.Storing;
//using Decoratid.Core.ValueOfing;
//using Decoratid.Extensions;
//using Decoratid.Idioms.Expiring;
//using Decoratid.Idioms.Testing;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;

//namespace Decoratid.Storidioms.Evicting
//{
//    public class Test : TestOf<IStore>
//    {
//        public Test()
//            : base(LogicOf<IStore>.New((x) =>
//            {
//                x.Clear();
//                var thing = AsId<string>.New("asId1");
//                var soid = thing.GetStoredObjectId();

//                //1. create an evicting store that always evicts (which means the item cannot be retrieved once it is committed, and it will live
//                //  in the store until the next poll @ .1 second intervals)
//                var store = x.EvictingInMemory(
//                LogicOfTo<IHasId, IExpirable>.New((it) =>
//                {
//                    return NaturalTrueExpirable.New();
//                }), 100); 

//                //wire to events
//                var isEvicted = false;
//                store.ItemEvicted +=  delegate(object sender, EventArgOf<Tuple<IHasId, IExpirable>> e)
//                {
//                    isEvicted = true;
//                };

//                //save 
//                store.SaveItem(thing);
//                Thread.Sleep(2000);

//                Assert.True(isEvicted);

//                //now pull from the store.  it should be null
//                var item = store.Get<AsId<string>>("asId1");
//                Assert.True(item == null);
//                store.Dispose();

//                //2. create an evicting store with a 3 second eviction
//                store = new NaturalInMemoryStore().EvictingInMemory(
//                LogicOfTo<IHasId, IExpirable>.New((it) =>
//                {
//                    return NaturalTrueExpirable.New().DecorateWithDateExpirable(DateTime.Now.AddSeconds(3));
//                }), 1000);
//                isEvicted = false;
//                store.ItemEvicted += delegate(object sender, EventArgOf<Tuple<IHasId, IExpirable>> e)
//                {
//                    isEvicted = true;
//                };

//                //save 
//                store.SaveItem(thing);

//                //now pull from the store.  it should exist
//                item = store.Get<AsId<string>>("asId1");
//                Assert.True(item != null);

//                //wait till expiry (3 seconds) and try to pull again
//                Thread.Sleep(4000);

//                Assert.True(isEvicted);

//                item = store.Get<AsId<string>>("asId1");
//                Assert.True(item == null);

//                store.Dispose();
//            }))
//        {
//        }
//    }
//}
