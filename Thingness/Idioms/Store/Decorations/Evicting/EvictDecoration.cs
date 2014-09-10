using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Thingness;
using Decoratid.Extensions;
using Decoratid.Logging;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Store.Implementations;
using Decoratid.Thingness.Idioms.Store.Decorations.Polling;
using Decoratid.Thingness.Idioms.Logics;
using Decoratid.Thingness.Idioms.ValuesOf;
using System.Runtime.Serialization;
using Decoratid.Serialization;
using Decoratid.Thingness.Decorations;
using Decoratid.Thingness.Idioms.ObjectGraph.Values;
using Decoratid.Thingness.Idioms.ObjectGraph;

namespace Decoratid.Thingness.Idioms.Store.Decorations.Evicting
{
    /// <summary>
    /// a store in which items that are added to it are given an expiry/eviction condition
    /// </summary>
    public interface IEvictingStore : IDecoratedStore
    {
        /// <summary>
        /// as items pass through the decorated store, they are assigned eviction info that needs to be stored somewhere.
        /// Not necessarily the same store as the decorated store
        /// </summary>
        IStore EvictionConditionStore { get; set; }

        /// <summary>
        /// factory used to create the default condition used in a simple Commit
        /// </summary>
        LogicOfTo<IHasId, ICondition> DefaultItemEvictionConditionFactory { get; set; }

        /// <summary>
        /// commit with the provided eviction condition for the data added
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="evictionCondition"></param>
        void Commit(ICommitBag cb, ICondition evictionCondition);

        /// <summary>
        /// fires when an item is evicted
        /// </summary>
        event EventHandler<EventArgOf<Tuple<IHasId, ICondition>>> ItemEvicted;

        /// <summary>
        /// force an eviction check
        /// </summary>
        void Evict();
    }

    /// <summary>
    /// Decorates with an eviction condition to each item in the registry
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class EvictDecoration : PollDecoration, IEvictingStore
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        /// <summary>
        /// ctor with no default eviction condition factory.  any items added will not be evicted without an eviction being set
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="backgroundIntervalMSecs"></param>
        public EvictDecoration(IStore evictionConditionStore,
            IStore decorated, double backgroundIntervalMSecs = 30000)
            : base(decorated)
        {
            this.EvictionConditionStore = evictionConditionStore;
            this.SetBackgroundAction(LogicOf<IStore>.New((reg) =>
            {
                this.Evict();
            }), backgroundIntervalMSecs);
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="defaultItemEvictionConditionFactory"></param>
        /// <param name="backgroundIntervalMSecs"></param>
        public EvictDecoration(IStore evictionConditionStore,
            IStore decorated,
            LogicOfTo<IHasId, ICondition> defaultItemEvictionConditionFactory,
            double backgroundIntervalMSecs = 10000)
            : base(decorated)
        {
            Condition.Requires(evictionConditionStore).IsNotNull();
            Condition.Requires(defaultItemEvictionConditionFactory).IsNotNull();
            Condition.Requires(backgroundIntervalMSecs).IsGreaterThan(0);

            this.EvictionConditionStore = evictionConditionStore;
            this.DefaultItemEvictionConditionFactory = defaultItemEvictionConditionFactory;
            this.SetBackgroundAction(LogicOf<IStore>.New((reg) =>
            {
                this.Evict();
            }), backgroundIntervalMSecs);
        }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new EvictDecoration(this.EvictionConditionStore, store, this.DefaultItemEvictionConditionFactory,
                this.BackgroundHost.BackgroundIntervalMSecs);

            return returnValue;
        }
        #endregion

        #region IHasHydrationMap
        public override IHydrationMap GetHydrationMap()
        {
            //get the inherited map
            var baseMap = base.GetHydrationMap();

            var map = new HydrationMapValueManager<EvictDecoration>();
            map.RegisterDefault("EvictionConditionStore", x => x.EvictionConditionStore, (x, y) => { x.EvictionConditionStore = y as IStore; });
            map.RegisterDefault("DefaultItemEvictionConditionFactory", x => x.DefaultItemEvictionConditionFactory, (x, y) => { x.DefaultItemEvictionConditionFactory = y as LogicOfTo<IHasId, ICondition>; });
            map.Maps.AddRange(baseMap.Maps);
            return map;
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

        #region IEvictingStore
        public IStore EvictionConditionStore { get; set; }
        /// <summary>
        /// factory used to create the default condition
        /// </summary>
        public LogicOfTo<IHasId, ICondition> DefaultItemEvictionConditionFactory { get; set; }
        /// <summary>
        /// the eviction event
        /// </summary>
        public event EventHandler<EventArgOf<Tuple<IHasId, ICondition>>> ItemEvicted;

        /// <summary>
        /// registers an item with a specific eviction condition.  If the condition is mutable, every touch/get of the item will result
        /// in a condition mutation.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="evictionCondition">may be null</param>
        public virtual void Commit(ICommitBag cb, ICondition evictionCondition)
        {
            lock (this._stateLock)
            {
                //commit first. if it kacks we don't do anything
                this.Decorated.Commit(cb);

                //build and commit the conditions
                CommitBag conditionCommitBag = new CommitBag();

                //foreach add, register a condition
                cb.ItemsToSave.WithEach(x =>
                {
                    //save the eviction condition in the eviction store, keyed by the storedobjectid of the item to save
                    var conditionToSave = ContextualAsId<StoredObjectId,ICondition>.New(x.GetStoredObjectId(), evictionCondition);
                    conditionCommitBag.MarkItemSaved(conditionToSave);
                });

                //foreach remove, remove a condition
                cb.ItemsToDelete.WithEach(x =>
                {
                    var conditionIdToDelete = ContextualAsId<StoredObjectId, ICondition>.CreateStoredObjectId(x);
                    conditionCommitBag.MarkItemDeleted(conditionIdToDelete);
                });

                this.EvictionConditionStore.Commit(conditionCommitBag);
            }
        }
        /// <summary>
        /// overrides, supplies the default condition 
        /// </summary>
        /// <param name="bag"></param>
        public override void Commit(ICommitBag cb)
        {
            lock (this._stateLock)
            {
                //commit first. if it kacks we don't do anything
                base.Commit(cb);

                //build and commit the conditions
                CommitBag conditionCommitBag = new CommitBag();

                //foreach add, register a condition
                if (this.DefaultItemEvictionConditionFactory != null)
                {
                    cb.ItemsToSave.WithEach(x =>
                    {
                        //generate the eviction condition
                        var evictionCondition = this.DefaultItemEvictionConditionFactory.CloneAndPerform(x.ValueOf());
                        //save the eviction condition in the eviction store, keyed by the storedobjectid of the item to save
                        var conditionToSave = ContextualAsId<StoredObjectId, ICondition>.New(x.GetStoredObjectId(), evictionCondition);
                        conditionCommitBag.MarkItemSaved(conditionToSave);

                    });
                }

                //foreach remove, remove a condition
                cb.ItemsToDelete.WithEach(x =>
                {
                    var conditionIdToDelete = ContextualAsId<StoredObjectId, ICondition>.CreateStoredObjectId(x);
                    conditionCommitBag.MarkItemDeleted(conditionIdToDelete);
                });

                this.EvictionConditionStore.Commit(conditionCommitBag);
            }
        }
        /// <summary>
        /// examines all eviction conditions and removes items that have an eviction condition of true.
        /// If an item's eviction condition is mutable, it will be mutated (eg. touched) on every get
        /// </summary>
        public void Evict()
        {
            List<ContextualAsId<StoredObjectId, ICondition>> itemsToEvict = new List<ContextualAsId<StoredObjectId, ICondition>>();

            //search the eviction store for evicts
            var evictions = this.EvictionConditionStore.GetAll<ContextualAsId<StoredObjectId, ICondition>>();

            foreach (var each in evictions)
            {
                if (each.Context != null)
                {
                    if (each.Context.Evaluate().GetValueOrDefault())
                    {
                        itemsToEvict.Add(each);
                    }
                }
            }

            //build deletes and commit them
            var evictCommitBag = new CommitBag();
            itemsToEvict.WithEach(x =>
            {
                evictCommitBag.MarkItemDeleted(x.Id);
            });
            this.Commit(evictCommitBag);

            //raise events (outside of state lock)
            itemsToEvict.WithEach(x =>
            {
                this.OnItemEvicted(x, x.Context);
            });
        }
        #endregion

        #region Events
        /// <summary>
        /// the eviction event raiser
        /// </summary>
        /// <param name="ci"></param>
        public void OnItemEvicted(IHasId iHasId, ICondition condition)
        {
            //skip if no listeners attached
            if (this.ItemEvicted == null)
                return;

            var args = this.ItemEvicted.BuildEventArgs(new Tuple<IHasId, ICondition>(iHasId, condition));
            //fire the event
            this.ItemEvicted(this, args);
        }
        #endregion

        #region Condition Store Registry
        /// <summary>
        /// mutates an item in the eviction condition store, and returns true if it's not flagged for eviction
        /// </summary>
        /// <param name="soId"></param>
        /// <returns></returns>
        protected bool TouchCondition(StoredObjectId soId)
        {
            if (soId == null)
                return false;

            bool isExpired = false;
            //get the item's condition from the condition store
            var cond = this.EvictionConditionStore.Get<ContextualAsId<StoredObjectId, ICondition>>(soId);

            //if the eviction condition is mutable - mutate it
            if (cond != null)
            {
                //set return value to current condition status 
                if (cond.Context != null)
                {
                    isExpired = cond.Context.Evaluate().GetValueOrDefault();
                }
                //mutate.  note this happens after checking the condition
                if (cond is IMutableCondition)
                {
                    IMutableCondition mutcond = (IMutableCondition)cond;
                    mutcond.Mutate();
                }
            }
            return !isExpired;
        }
        #endregion

        #region Overrides
        public override IHasId Get(IStoredObjectId soId)
        {
            var retval = this.Decorated.Get(soId);

            StoredObjectId id = StoredObjectId.New(soId.ObjectType, soId.ObjectId);
            if (this.TouchCondition(id))
                return retval;

            return null;
        }

        public override List<T> Search<T>(SearchFilter filter)
        {
            var list = this.Decorated.Search<T>(filter);

            List<T> returnValue = new List<T>();

            list.WithEach(x =>
            {
                if (this.TouchCondition(x.GetStoredObjectId()))
                    returnValue.Add(x);
            });

            return returnValue;
        }
        public override List<IHasId> GetAll()
        {
            var list = this.Decorated.GetAll();

            List<IHasId> returnValue = new List<IHasId>();

            list.WithEach(x =>
            {
                if (this.TouchCondition(x.GetStoredObjectId()))
                    returnValue.Add(x);
            });
            return returnValue;
        }

        #endregion
    }
}
