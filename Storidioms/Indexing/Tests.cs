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



                var thing1 = AsId<string>.New("asId1").HasContext("context").HasNameValue("property1", "property1value");
                var thing2 = AsId<string>.New("asId2").HasNameValue("property1", "property1value");
                var thing3 = AsId<string>.New("asId3").HasContext("context");

                Assert.True(thing1.HasDecorations<IHasContext, IHasNameValue, IHasId>());
                Assert.True(thing2.HasDecorations<IHasNameValue, IHasId>());
                Assert.True(thing3.HasDecorations<IHasContext, IHasId>());
                Assert.False(thing3.HasDecoration<IHasNameValue>());

                store.SaveItem(thing1);
                store.SaveItem(thing2);
                store.SaveItem(thing3);


                

                store.Dispose();
            }))
        {
        }
    }
}
