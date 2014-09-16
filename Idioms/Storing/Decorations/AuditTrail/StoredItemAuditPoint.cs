using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

namespace Decoratid.Idioms.Storing.Decorations.AuditTrail
{
    /// <summary>
    /// default item audit point definition (ie. the simplest, complete audit point def)
    /// </summary>
    public class StoredItemAuditPoint : IStoredItemAuditPoint, IHasId<DecoratidId>
    {
        #region Ctor
        public StoredItemAuditPoint(StoredItemAccessMode mode, IHasId item)
        {
            Condition.Requires(item).IsNotNull();

            this.Date = DateTime.UtcNow;
            this.Mode = mode;

            this.ObjRef = new StoredObjectId(item);
            this.Id = new DecoratidId();
            this.Item = item;

            if (mode == StoredItemAccessMode.Delete)
            {
                this.ObjRef = item as StoredObjectId;
            }
        }
        #endregion

        #region IStoredItemAuditPoint
        public DateTime Date { get; protected set; }
        public StoredItemAccessMode Mode { get; protected set; }
        public StoredObjectId ObjRef { get; protected set; }
        public DecoratidId Id { get; protected set; }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region Properties
        public IHasId Item { get; protected set; }
        #endregion

        #region Static Helpers
        public static Func<IHasId, StoredItemAccessMode, StoredItemAuditPoint> GetBuilderFunction()
        {
            return (item, mode) => { return new StoredItemAuditPoint(mode, item); };
        }
        #endregion

        #region Static Fluent

        #endregion
    }
}
