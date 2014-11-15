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
using Decoratid.Storidioms.StoreOf;
using Xunit;

namespace Decoratid.Storidioms.AuditTrail
{
    public class Test : TestOf<IStore>
    {
        public Test()
            : base(LogicOf<IStore>.New((x) =>
            {
                var thing = AsId<string>.New("asId1");
                var soid = thing.GetStoredObjectId();

                var store = x.BasicAudit(NaturalInMemoryStore.New().IsOf<StoredItemAuditPoint>());

                //do some stuff..all of this should be tracked
                store.SaveItem(thing);
                var auditItems = store.AuditStore.GetAll();
                Assert.True(auditItems[0].Mode == StoredItemAccessMode.Save && auditItems[0].ObjRef.Equals(soid));

                var clone = store.Get<AsId<string>>("asId1");
                auditItems = store.AuditStore.GetAll();
                Assert.True(auditItems[1].Mode == StoredItemAccessMode.Read && auditItems[1].ObjRef.Equals(soid));

                var list = store.Search<AsId<string>>(SearchFilter.New((o) => { return o.Id.Equals("asId1"); }));
                auditItems = store.AuditStore.GetAll();
                Assert.True(auditItems[2].Mode == StoredItemAccessMode.Read && auditItems[2].ObjRef.Equals(soid));

                var list2 = store.GetAll();
                auditItems = store.AuditStore.GetAll();
                Assert.True(auditItems[3].Mode == StoredItemAccessMode.Read && auditItems[3].ObjRef.Equals(soid));

                store.DeleteItem(thing.GetStoredObjectId());

                auditItems = store.AuditStore.GetAll();
                Assert.True(auditItems[4].Mode == StoredItemAccessMode.Delete && auditItems[4].ObjRef.Equals(soid));

            })) 
        { 
        }
    }

}
