using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Storidioms.AuditTrail
{
    /// <summary>
    /// the basic data required for a particular audit point (eg. when, who/what, how).  
    /// </summary>
    public interface IStoredItemAuditPoint : IHasId
    {
        DateTime Date { get; }
        StoredItemAccessMode Mode { get; }
        StoredObjectId ObjRef { get; }
    }

    /// <summary>
    /// marker interface indicating the store keeps an audit trail of all item changes
    /// </summary>
    public interface IAuditingStore<TAuditPoint> : IDecoratedStore
        where TAuditPoint : IStoredItemAuditPoint
    {
        /// <summary>
        /// the store containing the audit data
        /// </summary>
        IStoreOf<TAuditPoint> AuditStore { get; }

        /// <summary>
        /// the strategy used to convert the item into an audit point
        /// </summary>
        Func<IHasId, StoredItemAccessMode, TAuditPoint> AuditItemFactory { get; }
    }
}
