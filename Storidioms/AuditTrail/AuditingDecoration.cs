using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using Decoratid.Extensions;
using System.Runtime.Serialization;

namespace Decoratid.Storidioms.AuditTrail
{
    /// <summary>
    /// decorates a store such that audit points are written for every item action.  Decoration should be 
    /// applied after all data modifying decorations have been applied.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    [Serializable]
    public class AuditingDecoration<TAuditPoint> : DecoratedStoreBase, IAuditingStore<TAuditPoint>//, IHasHydrationMap
    where TAuditPoint : IStoredItemAuditPoint
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public AuditingDecoration(
            IStore decorated, IStoreOf<TAuditPoint> auditStore,
            Func<IHasId, StoredItemAccessMode, TAuditPoint> auditItemBuildStrategy)
            : base(decorated)
        {
            Condition.Requires(auditStore).IsNotNull();
            Condition.Requires(auditItemBuildStrategy).IsNotNull();
            this.AuditStore = auditStore;
            this.AuditItemFactory = auditItemBuildStrategy;
        }
        #endregion

        #region ISerializable
        protected AuditingDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.AuditItemFactory = (Func<IHasId, StoredItemAccessMode, TAuditPoint>)info.GetValue("AuditItemFactory", typeof(Func<IHasId, StoredItemAccessMode, TAuditPoint>));
            this.AuditStore = (IStoreOf<TAuditPoint>)info.GetValue("AuditStore", typeof(IStoreOf<TAuditPoint>));

        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AuditItemFactory", AuditItemFactory);
            info.AddValue("AuditStore", AuditStore);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IAuditingStore
        public IStoreOf<TAuditPoint> AuditStore { get; private set; }
        public Func<IHasId, StoredItemAccessMode, TAuditPoint> AuditItemFactory { get; private set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new AuditingDecoration<TAuditPoint>(store, this.AuditStore, this.AuditItemFactory);
            return returnValue;
        }
        #endregion

        #region Helpers
        private void BuildAndSaveAuditPoint(StoredItemAccessMode mode, IHasId item)
        {
            var auditPoint = this.AuditItemFactory(item, mode);
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

    public static class AuditingDecorationExtensions
    {
        /// <summary>
        /// adds an auditing decoration.  Apply after all data modifying decorations.
        /// </summary>
        /// <typeparam name="TAuditPoint"></typeparam>
        /// <param name="decorated"></param>
        /// <param name="auditStore"></param>
        /// <param name="auditItemBuildStrategy"></param>
        /// <returns></returns>
        public static AuditingDecoration<TAuditPoint> Audit<TAuditPoint>(this IStore decorated,
            IStoreOf<TAuditPoint> auditStore,
            Func<IHasId, StoredItemAccessMode, TAuditPoint> auditItemBuildStrategy)
                where TAuditPoint : IStoredItemAuditPoint
        {
            Condition.Requires(decorated).IsNotNull();
            return new AuditingDecoration<TAuditPoint>(decorated, auditStore, auditItemBuildStrategy);
        }
        /// <summary>
        /// adds an auditing decoration of StoredItemAuditPoint.  Apply after all data modifying decorations.
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="auditStore"></param>
        /// <returns></returns>
        public static AuditingDecoration<StoredItemAuditPoint> BasicAudit(this IStore decorated,
     IStoreOf<StoredItemAuditPoint> auditStore)
        {
            Condition.Requires(decorated).IsNotNull();
            return new AuditingDecoration<StoredItemAuditPoint>(decorated, auditStore,
                StoredItemAuditPoint.GetBuilderFunction() );
        }
    }
}
