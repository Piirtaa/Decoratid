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
using Decoratid.Idioms.HasBitsing;

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
                int numRecords = 1000000;
                Random rnd = new Random();
                
                for (int i = 0; i < numRecords; i++)
                {
                    var obj = i.BuildAsId();

                    //using a suddendeath iteration for each record, flip a coin and add a random namevalue
                    //try to get lucky 10 times.
                    //this will create discernable groupings in the distribution and predictability
                    //eg. probability of an item to have N NameValue pair decorations is = .5 ^ (n -1).
                    //because we're randomizing which namevalue pair decoration should also give us
                    //a flat bias at each quanta of grouping 
                    for (int j = 0; j < 10; j++ )
                    {
                        //coin flip
                        if (rnd.Next(2) == 0)
                            continue;

                        //pick a decoration from 0 to 22
                        var decNum = rnd.Next(20);
                        obj.HasNameValue(decNum.ToString(), decNum);//decorate
                    }

                    //then add name on a coin flip
                    if (rnd.Next(2) == 0)
                        obj.HasName(i.ToString());
                    
                    store.SaveItem(obj);
                }

                //add a needle to look for
                var needleObj = int.MaxValue.BuildAsId().HasName("root").HasNameValue("0", 0).HasNameValue("1", 1).HasNameValue("2", 2).HasNameValue("3", 3).HasNameValue("4", 4).HasNameValue("5", 5);
                
                //use this as a mask
                var mask = store.StoreOfIndices.IndexFactory.GenerateIndex(needleObj);

                //now search for stuff
                var matches = store.SearchIndex(mask.BuildANDLogic());

                store.Dispose();
            }))
        {
        }
    }
}
