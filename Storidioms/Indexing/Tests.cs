using Decoratid.Core.Conditional;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Core.ValueOfing;
using Decoratid.Core.Contextual;
using Decoratid.Core.Decorating;
using Decoratid.Extensions;
using Decoratid.Idioms.Expiring;
using Decoratid.Idioms.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Decoratid.Storidioms.Indexing
{
    public class Test : TestOf<IStore>
    {
        public Test()
            : base(LogicOf<IStore>.New((x) =>
            {
                x.Clear();

                //add a bunch of decorations and then search for them
                IndexingDecoration store = x.WithIndex();

                //bit 0- IHasContext
                //since we're not supplying an index, it assumes it's the next one
                store.StoreOfIndices.IndexFactory.SetBitLogic("hascontext", (o) => { return o is IHasContext; });
                //bit 1 - IHasNameValue
                store.StoreOfIndices.IndexFactory.SetBitLogic("hasnamevalue", (o) => { return o is IHasNameValue; });
                //bit 2 - IHasName
                store.StoreOfIndices.IndexFactory.SetBitLogic("hasname", (o) => { return o is IHasName; });

                for (int i = 0; i < 20; i++)
                {
                    //bit 3 + i - IHasNameValue - name of i
                    store.StoreOfIndices.IndexFactory.SetBitLogic("hasnamevalue" + i, (o) => { return o is IHasNameValue && (o as IHasNameValue).Name == i.ToString(); });
                }
                
                //this gives us a few decorations to search thru. now generate mock data
                int numRecords = 10000000;
                Random rnd = new Random();
                
                for (int i = 0; i < numRecords; i++)
                {
                    var obj = i.BuildAsId();

                    //using a suddendeath iteration, flip a coin and add a random namevalue
                    for (int j = 0; j < 10; j++ )
                    {
                        //coin flip
                        if (rnd.Next(2) == 0)
                            continue;

                        //pick a decoration from 0 to 22
                        var decNum = rnd.Next(20);
                        obj.HasNameValue(decNum.ToString(), decNum);//decorate
                    }

                    //coin flip
                    if (rnd.Next(2) == 0)
                        obj.HasName(i.ToString());
                    
                    store.SaveItem(obj);
                }

                //now search for stuff

                

                store.Dispose();
            }))
        {
        }
    }
}
