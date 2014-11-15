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

namespace Decoratid.Storidioms.AuditTrail
{
    public class Test : TestOf<IStore>
    {
        public Test()
            : base(LogicOf<IStore>.New((x) =>
            {
                var thing4 = AsId<string>.New("asId1");

                var store = NaturalInMemoryStore.New().DecorateWithBasicAuditing(NaturalInMemoryStore.New().IsOf<StoredItemAuditPoint>());

                //do some stuff..all of this should be tracked
                store.SaveItem(thing4);
                var auditItems = store.AuditStore.GetAll();
                Assert.True(auditItems[0].Mode == StoredItemAccessMode.Save && auditItems[0].ObjRef.Equals(thing4.GetStoredObjectId()));

                var clone = store.Get<AsId<string>>("asId1");
                auditItems = store.AuditStore.GetAll();
                Assert.True(auditItems[1].Mode == StoredItemAccessMode.Read && auditItems[1].ObjRef.Equals(thing4.GetStoredObjectId()));

                var list = store.Search<AsId<string>>(SearchFilter.New((x) => { return x.Id.Equals("asId1"); }));
                auditItems = store.AuditStore.GetAll();
                Assert.True(auditItems[2].Mode == StoredItemAccessMode.Read && auditItems[2].ObjRef.Equals(thing4.GetStoredObjectId()));

                var list2 = store.GetAll();
                auditItems = store.AuditStore.GetAll();
                Assert.True(auditItems[3].Mode == StoredItemAccessMode.Read && auditItems[3].ObjRef.Equals(thing4.GetStoredObjectId()));

                store.DeleteItem(thing4.GetStoredObjectId());

                auditItems = store.AuditStore.GetAll();
                Assert.True(auditItems[4].Mode == StoredItemAccessMode.Delete && auditItems[4].ObjRef.Equals(thing4.GetStoredObjectId()));

            })) 
        { 
        }
    }

}
