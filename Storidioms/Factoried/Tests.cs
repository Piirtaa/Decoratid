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

namespace Decoratid.Storidioms.Factoried
{
    public class Test : TestOf<IStore>
    {
        public Test()
            : base(LogicOf<IStore>.New((x) =>
            {
                var store = x.HasFactory(LogicOfTo<IStoredObjectId, IHasId>.New((soId) =>
                {

                    //the factory produces AsId<string> only
                    if (soId.ObjectType.Equals(typeof(AsId<string>)))
                    {
                        return AsId<string>.New(soId.ObjectId.ToString());
                    }

                    return null;
                }));


                //pull from the store, which is empty.  it should factory the item up
                var item = store.Get<AsId<string>>("asId1");
                Assert.True(item != null);

                //delete it 
                store.DeleteItem(item.GetStoredObjectId());

                item = store.Get<AsId<string>>("asId1");
                Assert.True(item != null);

                //cleanup
                store.DeleteItem(item.GetStoredObjectId());
            }))
        {
        }
    }
}
