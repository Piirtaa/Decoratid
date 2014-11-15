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
using Xunit;

namespace Decoratid.Storidioms.StoreOf
{
    #region Test Types
    public class BaseThing : IHasId<string>
    {
        public string Id { get; set; }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        public string Data;
    }
    public class DerivedThing : BaseThing
    {
        public string Data1;
    }
    public class DerivedDerivedThing : DerivedThing
    {
        public string Data2;
    }
    #endregion

    public class StoreOfTest : TestOf<IStore>
    {
        public StoreOfTest()
            : base(LogicOf<IStore>.New((x) =>
            {
                var store = x.IsOf<BaseThing>();

                BaseThing thing1 = new BaseThing() { Data = "data", Id = "thing1" };
                DerivedThing thing2 = new DerivedThing() { Data = "data", Data1 = "data1", Id = "thing2" };
                DerivedDerivedThing thing3 = new DerivedDerivedThing() { Data = "data", Data1 = "data1", Data2 = "data2", Id = "thing3" };
                var thing4 = AsId<string>.New("asId1");

                store.SaveItem(thing1);
                store.SaveItem(thing2);
                store.SaveItem(thing3);

                Assert.Throws<InvalidOperationException>(() =>
                {
                    store.SaveItem(thing4);
                });

                store.DeleteItems(thing1.GetStoredObjectId(), thing2.GetStoredObjectId(), thing3.GetStoredObjectId());
            }))
        {
        }
    }

    public class StoreOfExactlyTest : TestOf<IStore>
    {
        public StoreOfExactlyTest()
            : base(LogicOf<IStore>.New((x) =>
            {
                var store = x.IsExactlyOf<BaseThing>();

                BaseThing thing1 = new BaseThing() { Data = "data", Id = "thing1" };
                DerivedThing thing2 = new DerivedThing() { Data = "data", Data1 = "data1", Id = "thing2" };
                DerivedDerivedThing thing3 = new DerivedDerivedThing() { Data = "data", Data1 = "data1", Data2 = "data2", Id = "thing3" };
                var thing4 = AsId<string>.New("asId1");

                store.SaveItem(thing1);
                Assert.Throws<InvalidOperationException>(() =>
                {
                    store.SaveItem(thing2);
                });
                Assert.Throws<InvalidOperationException>(() =>
                {
                    store.SaveItem(thing3);
                });
                Assert.Throws<InvalidOperationException>(() =>
                {
                    store.SaveItem(thing4);
                });

                store.DeleteItems(thing1.GetStoredObjectId());
            }))
        {
        }
    }

    public class StoreOfUniqueIdTest : TestOf<IStore>
    {
        public StoreOfUniqueIdTest()
            : base(LogicOf<IStore>.New((x) =>
            {
                var store = x.IsOfUniqueId<BaseThing>();

                BaseThing thing1 = new BaseThing() { Data = "data", Id = "thing1" };
                DerivedThing thing2 = new DerivedThing() { Data = "data", Data1 = "data1", Id = "thing2" };
                DerivedDerivedThing thing3 = new DerivedDerivedThing() { Data = "data", Data1 = "data1", Data2 = "data2", Id = "thing1" };
                var thing4 = AsId<string>.New("asId1");

                store.SaveItem(thing1);
                store.SaveItem(thing2);
                Assert.Throws<InvalidOperationException>(() =>
                {
                    store.SaveItem(thing3);
                });
                Assert.Throws<InvalidOperationException>(() =>
                {
                    store.SaveItem(thing4);
                });

                store.DeleteItems(thing1.GetStoredObjectId(), thing2.GetStoredObjectId());
            }))
        {
        }
    }
    //public class Test : TestOf<IStore>
    //{
    //    public Test()
    //        : base(LogicOf<IStore>.New((x) =>
    //        {

    //            ////create a store of contextual things 
    //            //var storeOf = NaturalInMemoryStore.New().IsOf<IContextualHasId>();

    //            //var obj1 = AsId<string>.New("obj1").AddContext("context");
    //            //var obj1Id = obj1.GetStoredObjectId();

    //            ////save and validate
    //            //storeOf.SaveItem(obj1);
    //            //Condition.Requires(storeOf.Contains(obj1Id)).IsTrue();

    //            //var obj2 = ContextualId<string, string>.New("obj2", "context");
    //            //var obj2Id = obj2.GetStoredObjectId();

    //            ////save and validate
    //            //storeOf.SaveItem(obj2);
    //            //Condition.Requires(storeOf.Contains(obj2Id)).IsTrue();

    //            ////now remove 



    //        }))
    //    {
    //    }
    //}
}
