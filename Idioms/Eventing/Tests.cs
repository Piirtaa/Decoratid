using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Core.ValueOfing;
using Decoratid.Extensions;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Decoratid.Idioms.Logging;

namespace Decoratid.Idioms.Eventing
{
    public class ConditionTest : TestOf<ICondition>
    {
        public ConditionTest()
            : base(LogicOf<ICondition>.New((x) =>
            {
                bool checkVal = false;
                var newX = x.Eventing();
                newX.Evaluated += delegate(object sender, EventArgOf<ICondition> e)
                {
                    checkVal = true;
                };
                newX.Evaluate();
                Thread.Sleep(50);
                Condition.Requires(checkVal).IsTrue();
            })) 
        { 
        }
    }

    public class ValueOfTest<T> : TestOf<IValueOf<T>>
    {
        public ValueOfTest()
            : base(LogicOf<IValueOf<T>>.New((x) =>
            {
                bool checkVal = false;
                var newX = x.Eventing();
                newX.Evaluated += delegate(object sender, EventArgOf<IValueOf<T>> e)
                {
                    checkVal = true;
                };
                newX.GetValue();
                Thread.Sleep(50);
                Condition.Requires(checkVal).IsTrue();


            }))
        {
        }
    }

    public class LogicTest : TestOf<ILogic>
    {
        public LogicTest()
            : base(LogicOf<ILogic>.New((x) =>
            {
                bool checkVal = false;
                var newX = x.Eventing();
                newX.Performed += delegate(object sender, EventArgOf<ILogic> e)
                {
                    checkVal = true;
                };
                newX.Perform();
                Thread.Sleep(50);
                Condition.Requires(checkVal).IsTrue();
            }))
        {
        }
    }

    public class Test : TestOf<IStore>
    {
        public Test()
            : base(LogicOf<IStore>.New((x) =>
            {

                var thing = AsId<string>.New("asId1");
                var filteredThing = AsId<string>.New("asId2_filtered");

                //1. test save events - don't save items with id's ending in "_filtered"
                var store = x.DecorateWithEvents(StoreLogger.NewInMemory());

                store.CommitOperationIntercept.AddNextIntercept("intercept1", (o)=>{
                    CommitBag newBag = new CommitBag();
                    var oldBag = o;
                    oldBag.ItemsToSave.WithEach(saveItem =>
                    {
                        string id = saveItem.Id.ToString();
                        if (id.EndsWith("_filtered"))
                        {
                            //don't add to new commitbag
                        }
                        else
                        {
                            newBag.MarkItemSaved(saveItem);
                        }
                    });
                    return newBag;
                }
                , null, null, null, null);

                //event flags
                bool saveFlag = false;
                bool saveFiltFlag = false;
                store.ItemSaved += (sender, e) =>
                {
                    saveFlag = true;
                };
                store.ItemSavedFiltered += (sender, e) =>
                {
                    saveFiltFlag = true;
                };

                store.SaveItem(thing);
                store.SaveItem(filteredThing);
                Assert.True(saveFiltFlag && saveFlag);

                //2. test delete events - don't delete items with ids ending in "_filtered"
                store = x.DecorateWithEvents(StoreLogger.NewInMemory());
                store.CommitOperationIntercept.AddNextIntercept("intercept1",
                (o) =>
                {
                    CommitBag newBag = new CommitBag();
                    var oldBag = o;

                    oldBag.ItemsToDelete.WithEach(delItem =>
                    {
                        string id = delItem.ObjectId.ToString();
                        if (id.EndsWith("_filtered"))
                        {
                            //don't add to new commitbag
                        }
                        else
                        {
                            newBag.MarkItemDeleted(delItem);
                        }
                    });
                    return newBag;

                }, null, null, null, null);

                bool delFlag = false;
                bool delFiltFlag = false;
                store.ItemDeleted += (sender, e) =>
                {
                    delFlag = true;
                };
                store.ItemDeletedFiltered += (sender, e) =>
                {
                    delFiltFlag = true;
                };
                store.SaveItem(thing);
                store.SaveItem(filteredThing);
                store.DeleteItem(thing.GetStoredObjectId());
                store.DeleteItem(filteredThing.GetStoredObjectId());
                Assert.True(delFiltFlag && delFlag);

                //3. test retrieve events - don't get items with ids ending in "_filtered"
                store = x.DecorateWithEvents(StoreLogger.NewInMemory());

                //do get all first
                store.GetAllOperationIntercept.AddNextIntercept("intercept1", null, null, null,
                (o) =>
                {
                    List<IHasId> newList = new List<IHasId>();
                    var oldList = o;

                    oldList.WithEach(item =>
                    {
                        if (!item.Id.ToString().EndsWith("_filtered"))
                            newList.Add(item);
                    });
                    return newList;

                },  null);
                bool retFlag = false;
                bool retFiltFlag = false;
                store.ItemRetrieved += (sender, e) =>
                {
                    retFlag = true;
                };
                store.ItemRetrievedFiltered += (sender, e) =>
                {
                    retFiltFlag = true;
                };

                store.SaveItem(thing);
                store.SaveItem(filteredThing);
                var items = store.GetAll();
                Assert.True(retFiltFlag && retFlag);

                //now test Search
                retFiltFlag = false;
                retFlag = false;

                store.SearchOperationIntercept.AddNextIntercept("intercept2", null, null, null,
                (o) =>
                {
                    List<IHasId> newList = new List<IHasId>();
                    var oldList = o;

                    oldList.WithEach(item =>
                    {
                        if (!item.Id.ToString().EndsWith("_filtered"))
                            newList.Add(item);
                    });
                    return newList;

                }, null);

                var filter = SearchFilterOf<AsId<string>>.NewOf((o) => { return true; });
                var searchList = store.Search<AsId<string>>(filter);
                Assert.True(retFiltFlag && retFlag);

                //now test get
                store.GetOperationIntercept.AddNextIntercept("intercept3", null, null, null,
                    (o) =>
                    {
                        IHasId newObj = null;
                        IHasId oldObj = o;
                        if (!oldObj.Id.ToString().EndsWith("_filtered"))
                            newObj = oldObj;
                        return newObj;
                    }, null);

                retFiltFlag = false;
                retFlag = false;
                store.Get(thing.GetStoredObjectId());
                store.Get(filteredThing.GetStoredObjectId());
                Assert.True(retFiltFlag && retFlag);
            }))
        {
        }
    }
}
