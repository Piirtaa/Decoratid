//using CuttingEdge.Conditions;
//using Decoratid.Core.Conditional;
//using Decoratid.Core.Logical;
//using Decoratid.Core.Storing;
//using Decoratid.Core.ValueOfing;
//using Decoratid.Extensions;
//using Decoratid.Idioms.Testing;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Decoratid.Idioms.Logging;
//using Decoratid.Core.Identifying;
//using Xunit;

//namespace Decoratid.Idioms.Intercepting
//{

//    public class Test : TestOf<IStore>
//    {
//        public Test()
//            : base(LogicOf<IStore>.New((x) =>
//            {
 //               x.Clear();
//                var store = x.Intercepting(FileLoggerUtil.GetFileLogger("storeinttest.txt"));

//                //build a commit interception that:
//                //decorates the arg by appending the character A to the id
//                //validates the arg by checking for the A
//                store.CommitOperationIntercept.AddNextIntercept("intercept1",
//                (o) =>
//                {
//                    CommitBag newBag = new CommitBag();
//                    var oldBag = o;

//                    oldBag.ItemsToSave.WithEach(saveItem =>
//                    {
//                        if (saveItem is AsId<string>)
//                        {
//                            var newItem = AsId<string>.New(saveItem.Id.ToString() + "A");
//                            newBag.MarkItemSaved(newItem);
//                        }
//                    });
//                    return newBag;

//                }, (o) =>
//                {
//                    var oldBag = o;
//                    oldBag.ItemsToSave.WithEach(saveItem =>
//                    {
//                        if (saveItem is AsId<string>)
//                        {
//                            var id = saveItem.Id.ToString();
//                            Assert.True(id.EndsWith("A"));
//                        }
//                    });
//                }, null, null, null);

//                var thing4 = AsId<string>.New("asId1");
//                store.SaveItem(thing4);

//                //pull from the store, which is empty.  it should factory the item up
//                var clone = store.Get<AsId<string>>("asId1A");
//                Assert.True(clone != null);

//                //TODO: test all intercepts, not just Commit

//                store.Dispose();
//            }))
//        {
//        }
//    }
//}
