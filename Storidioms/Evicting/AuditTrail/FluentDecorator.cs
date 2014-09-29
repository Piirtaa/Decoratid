using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Storidioms.AuditTrail;
using Decoratid.Storidioms.StoreOf;

namespace Decoratid.Core.Storing
{
    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Core.Storing namespace.
    /// </summary>
    public static partial class FluentDecorator
    {
        /// <summary>
        /// adds an auditing decoration.  Apply after all data modifying decorations.
        /// </summary>
        /// <typeparam name="TAuditPoint"></typeparam>
        /// <param name="decorated"></param>
        /// <param name="auditStore"></param>
        /// <param name="auditItemBuildStrategy"></param>
        /// <returns></returns>
        public static AuditingDecoration<TAuditPoint> DecorateWithAuditing<TAuditPoint>(this IStore decorated,
            IStoreOf<TAuditPoint> auditStore,
            Func<IHasId, StoredItemAccessMode, TAuditPoint> auditItemBuildStrategy)
                where TAuditPoint : IStoredItemAuditPoint
        {
            Condition.Requires(decorated).IsNotNull();
            return new AuditingDecoration<TAuditPoint>(auditStore, auditItemBuildStrategy, decorated);
        }
        /// <summary>
        /// adds an auditing decoration of StoredItemAuditPoint.  Apply after all data modifying decorations.
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="auditStore"></param>
        /// <returns></returns>
        public static AuditingDecoration<StoredItemAuditPoint> DecorateWithBasicAuditing(this IStore decorated,
     IStoreOf<StoredItemAuditPoint> auditStore)
        {
            Condition.Requires(decorated).IsNotNull();
            return new AuditingDecoration<StoredItemAuditPoint>(auditStore,
                StoredItemAuditPoint.GetBuilderFunction(), decorated);
        }
    }



}
