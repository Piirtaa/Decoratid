using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Storing.Decorations.StoreOf;
using Decoratid.Extensions;
using Decoratid.Thingness;
using System.Runtime.Serialization;
using System.Reflection;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Core.Storing.Decorations.AuditTrail
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
        Func<IHasId, StoredItemAccessMode, TAuditPoint> AuditItemBuildStrategy { get; }
    }

    /// <summary>
    /// decorates a store such that audit points are written for every item action.  Decoration should be 
    /// applied after all data modifying decorations have been applied.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class AuditingDecoration<TAuditPoint> : DecoratedStoreBase, IAuditingStore<TAuditPoint>, IHasHydrationMap
    where TAuditPoint : IStoredItemAuditPoint
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public AuditingDecoration(IStoreOf<TAuditPoint> auditStore,
            Func<IHasId, StoredItemAccessMode, TAuditPoint> auditItemBuildStrategy,
            IStore decorated)
            : base(decorated)
        {
            Condition.Requires(auditStore).IsNotNull();
            Condition.Requires(auditItemBuildStrategy).IsNotNull();
            this.AuditStore = auditStore;
            this.AuditItemBuildStrategy = auditItemBuildStrategy;
        }
        #endregion

        #region IAuditingStore
        public IStoreOf<TAuditPoint> AuditStore { get; private set; }
        public Func<IHasId, StoredItemAccessMode, TAuditPoint> AuditItemBuildStrategy { get; private set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new AuditingDecoration<TAuditPoint>(this.AuditStore, this.AuditItemBuildStrategy, store);
            return returnValue;
        }
        #endregion

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var hydrationMap = new HydrationMapValueManager<AuditingDecoration<TAuditPoint>>();
            hydrationMap.RegisterDefault("AuditStore", x => x.AuditStore, (x, y) => { x.AuditStore = y as IStoreOf<TAuditPoint>; });
            hydrationMap.RegisterDefault("AuditItemBuildStrategy", x => x.AuditItemBuildStrategy, (x, y) => { x.AuditItemBuildStrategy = y as Func<IHasId, StoredItemAccessMode, TAuditPoint>; });
            return hydrationMap;
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return this.GetHydrationMap().DehydrateValue(this, uow);
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {
            this.GetHydrationMap().HydrateValue(this, text, uow);
        }
        #endregion

        #region Helpers
        private void BuildAndSaveAuditPoint(StoredItemAccessMode mode, IHasId item)
        {
            var auditPoint = this.AuditItemBuildStrategy(item, mode);
            this.AuditStore.SaveItem(auditPoint);
        }
        #endregion

        #region Overrides
        //override all of the store functions, adding audit 
        public override IHasId Get(IStoredObjectId soId)
        {
            var val = this.Decorated.Get(soId);

            this.BuildAndSaveAuditPoint(StoredItemAccessMode.Read, val);

            return val;
        }
        public override List<T> Search<T>(SearchFilter filter)
        {
            var val = this.Decorated.Search<T>(filter);

            val.WithEach(x =>
            {
                this.BuildAndSaveAuditPoint(StoredItemAccessMode.Read, x);
            });

            return val;
        }
        public override List<IHasId> GetAll()
        {
            var val = this.Decorated.GetAll();

            val.WithEach(x =>
            {
                this.BuildAndSaveAuditPoint(StoredItemAccessMode.Read, x);
            });

            return val;
        }
        public override void Commit(ICommitBag bag)
        {
            this.Decorated.Commit(bag);

            bag.ItemsToSave.WithEach(x =>
            {
                this.BuildAndSaveAuditPoint(StoredItemAccessMode.Save, x);
            });

            bag.ItemsToDelete.WithEach(x =>
            {
                this.BuildAndSaveAuditPoint(StoredItemAccessMode.Delete, x);
            });
        }
        #endregion
    }
}
