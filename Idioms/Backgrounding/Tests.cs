using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core.Identifying;
using Xunit;

namespace Decoratid.Idioms.Backgrounding
{
    public class ConditionTest : TestOf<ICondition>
    {
        public ConditionTest()
            : base(LogicOf<ICondition>.New((x) =>
            {
                //give this thing polling, and have the polling job turn a switch on
                bool switchVal = false;

                var newX = x.Polls();
                newX.SetBackgroundAction(LogicOf<ICondition>.New((thing) =>
                {
                    switchVal = true;
                }), 1000);

                //wait 2 seconds and the switch should be true
                Thread.Sleep(2000);
                Condition.Requires(switchVal).IsTrue();
            })) 
        { 
        }
    }

    public class ValueOfTest : TestOf<IValueOf<string>>
    {
        public ValueOfTest()
            : base(LogicOf<IValueOf<string>>.New((x) =>
            {
                //give this thing polling, and have the polling job turn a switch on
                bool switchVal = false;

                var newX = x.Polls();
                newX.SetBackgroundAction(LogicOf<IValueOf<string>>.New((thing) =>
                {
                    switchVal = true;
                }), 1000);

                //wait 2 seconds and the switch should be true
                Thread.Sleep(2000);
                Condition.Requires(switchVal).IsTrue();

            }))
        {
        }
    }

    public class LogicTest : TestOf<ILogic>
    {
        public LogicTest()
            : base(LogicOf<ILogic>.New((x) =>
            {
                //give this thing polling, and have the polling job turn a switch on
                bool switchVal = false;

                var newX = x.Polls();
                newX.SetBackgroundAction(LogicOf<ILogic>.New((thing) =>
                {
                    switchVal = true;
                }), 1000);

                //wait 2 seconds and the switch should be true
                Thread.Sleep(2000);
                Condition.Requires(switchVal).IsTrue();

            }))
        {
        }
    }

    public class Test : TestOf<IStore>
    {
        public Test()
            : base(LogicOf<IStore>.New((x) =>
            {
                //create store that polls every 5 secs
                var store = x.Polls();
                store.SetBackgroundAction(LogicOf<IStore>.New((s) =>
                {
                    //the poll action deletes all items in the store
                    var items = x.GetAll();
                    items.WithEach(item =>
                    {
                        x.DeleteItem(item.GetStoredObjectId());
                    });

                }), 5000);

                var thing4 = AsId<string>.New("asId1");
                store.SaveItem(thing4);

                //pull from the store, which is empty.  it should factory the item up
                var clone = store.Get<AsId<string>>("asId1");
                Assert.True(clone != null);

                //wait 7 secs
                Thread.Sleep(7000);

                //now try to read it again - it should be gone
                clone = store.Get<AsId<string>>("asId1");
                Assert.True(clone == null);

            })) { }
    }

}
