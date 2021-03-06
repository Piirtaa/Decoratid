﻿using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Contextual;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Core.ValueOfing;
using Decoratid.Extensions;
using Decoratid.Idioms.Backgrounding;
using Decoratid.Idioms.Expiring;
using Decoratid.Idioms.Polyfacing;
using Decoratid.Idioms.Touching;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Decoratid.Storidioms.Evicting
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
        IStore ExpirableStore { get; set; }

        /// <summary>
        /// factory used to create the default condition used in a simple Commit
        /// </summary>
        LogicOfTo<IHasId, IExpirable> ExpirableFactory { get; set; }

        /// <summary>
        /// commit with the provided eviction condition for the data added
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="evictionCondition"></param>
        void Commit(ICommitBag cb, IExpirable expirable);

        /// <summary>
        /// fires when an item is evicted
        /// </summary>
        event EventHandler<EventArgOf<Tuple<IHasId, IExpirable>>> ItemEvicted;

        /// <summary>
        /// force an eviction check
        /// </summary>
        void Evict();
    }

    /// <summary>
    /// a store that keeps an eviction condition for each item, and on a background job, performs eviction of indicated items
    /// </summary>
    /// <remarks>
    /// internally decorates with a polling decoration used to run Evict()
    /// </remarks>
    public class EvictingDecoration : DecoratedStoreBase, IEvictingStore
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public EvictingDecoration(IStore decorated,
            IStore expirableStore,
            LogicOfTo<IHasId, IExpirable> expirableFactory,
            double backgroundIntervalMSecs = 30000)
            : base(decorated.Polls())
        {
            Condition.Requires(expirableStore).IsNotNull();
            Condition.Requires(expirableFactory).IsNotNull();

            this.ExpirableStore = expirableStore;
            this.ExpirableFactory = expirableFactory;

            var poller = this.GetPoll();
            poller.SetBackgroundAction(LogicOf<IStore>.New((reg) =>
            {
                this.Evict();
            }), backgroundIntervalMSecs);
        }
        #endregion

        #region ISerializable
        protected EvictingDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //this.Factory = (LogicOfTo<IStoredObjectId, IHasId>)info.GetValue("Factory", typeof(LogicOfTo<IStoredObjectId, IHasId>));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.AddValue("Factory", Factory);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new EvictingDecoration(this.ExpirableStore, store, this.ExpirableFactory);

            return returnValue;
        }
        #endregion

        #region IEvictingStore
        public IStore ExpirableStore { get; set; }
        /// <summary>
        /// factory used to create the default condition
        /// </summary>
        public LogicOfTo<IHasId, IExpirable> ExpirableFactory { get; set; }
        /// <summary>
        /// the eviction event
        /// </summary>
        public event EventHandler<EventArgOf<Tuple<IHasId, IExpirable>>> ItemEvicted;

        /// <summary>
        /// registers an item with a specific eviction condition.  If the condition is mutable, every touch/get of the item will result
        /// in a condition mutation.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="evictionCondition">may be null</param>
        public virtual void Commit(ICommitBag cb, IExpirable evictionCondition)
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
                    var conditionToSave = x.GetStoredObjectId().BuildAsId().HasContext(evictionCondition);
                    conditionCommitBag.MarkItemSaved(conditionToSave);
                });

                //foreach remove, remove a condition
                cb.ItemsToDelete.WithEach(x =>
                {
                    var delId = StoredObjectId.New(typeof(ContextualIHasIdDecoration), x);
                    conditionCommitBag.MarkItemDeleted(delId);
                });

                this.ExpirableStore.Commit(conditionCommitBag);
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
                if (this.ExpirableFactory != null)
                {
                    cb.ItemsToSave.WithEach(x =>
                    {
                        //generate the eviction condition
                        var evictionConditionLogic = this.ExpirableFactory.Perform(x) as LogicOfTo<IHasId, IExpirable>;

                        //save the eviction condition in the eviction store, keyed by the storedobjectid of the item to save
                        var conditionToSave = x.GetStoredObjectId().BuildAsId().HasContext(evictionConditionLogic.Result);
                        //var soid = conditionToSave.GetStoredObjectId();
                        //var soid2 = StoredObjectId.New(typeof(ContextualIHasIdDecoration), x.GetStoredObjectId());
                        //bool isEq = soid.Equals(soid2);
                        conditionCommitBag.MarkItemSaved(conditionToSave);

                    });
                }

                //foreach remove, remove a condition
                cb.ItemsToDelete.WithEach(x =>
                {
                    var delId = StoredObjectId.New(typeof(ContextualIHasIdDecoration), x);
                    conditionCommitBag.MarkItemDeleted(delId);
                });

                this.ExpirableStore.Commit(conditionCommitBag);
            }
        }
        /// <summary>
        /// examines all eviction conditions and removes items that have an eviction condition of true.
        /// If an item's eviction condition is mutable, it will be mutated (eg. touched) on every get
        /// </summary>
        public void Evict()
        {
            List<ContextualIHasIdDecoration> itemsToEvict = new List<ContextualIHasIdDecoration>();

            //search the eviction store for evicts
            var evictions = this.ExpirableStore.GetAll();

            foreach (var each in evictions)
            {
                var eachItem = each as ContextualIHasIdDecoration;
                if (eachItem != null && eachItem.Context != null)
                {
                    IExpirable exp = eachItem.Context as IExpirable;
                    if (exp.IsExpired())
                    {
                        itemsToEvict.Add(eachItem);
                    }
                }
            }

            //build deletes and commit them
            var mainCommitBag = new CommitBag();
            var expCommitBag = new CommitBag(); 
            itemsToEvict.WithEach(x =>
            {
                StoredObjectId soid = x.Id as StoredObjectId;
                mainCommitBag.MarkItemDeleted(soid);
                expCommitBag.MarkItemDeleted(x.GetStoredObjectId());
            });

            //remove the item specified by the expirable policy.  
            this.Commit(mainCommitBag);

            //now delete from the expiry store
            this.ExpirableStore.Commit(expCommitBag);
            
            //raise events (outside of state lock)
            itemsToEvict.WithEach(x =>
            {
                this.OnItemEvicted(x, x.Context as IExpirable);
            });
        }
        #endregion

        #region Events
        /// <summary>
        /// the eviction event raiser
        /// </summary>
        /// <param name="ci"></param>
        public void OnItemEvicted(IHasId iHasId, IExpirable condition)
        {
            //skip if no listeners attached
            if (this.ItemEvicted == null)
                return;

            var args = this.ItemEvicted.BuildEventArgs(new Tuple<IHasId, IExpirable>(iHasId, condition));
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
        protected bool TouchExpirable(StoredObjectId soId)
        {
            if (soId == null)
                return false;

            bool isExpired = false;
            //get the item's condition from the condition store
            var record = this.ExpirableStore.Get<ContextualId<StoredObjectId, IExpirable>>(soId);

            //if the eviction condition is touchable, touch it
            if (record != null && record.Context != null)
            {
                isExpired = record.Context.IsExpired();

                //if the expirable is touchable, touch it
                if (record.Context is ITouchable)
                {
                    ITouchable touch = (ITouchable)record.Context;
                    touch.Touch();
                }
                else if (record.Context is IPolyfacing) //or if it's polyfacing with a touchable face
                {
                    IPolyfacing poly = (IPolyfacing)record.Context;
                    var touch = poly.RootFace.AsHasTouchable();
                    if (touch != null)
                    {
                        touch.Touch();
                    }
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
            if (this.TouchExpirable(id))
                return retval;

            return null;
        }

        public override List<IHasId> Search(LogicOfTo<IHasId,bool> filter)
        {
            var list = this.Decorated.Search(filter);

            List<IHasId> returnValue = new List<IHasId>();

            list.WithEach(x =>
            {
                if (this.TouchExpirable(x.GetStoredObjectId()))
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
                if (this.TouchExpirable(x.GetStoredObjectId()))
                    returnValue.Add(x);
            });
            return returnValue;
        }

        #endregion

        #region Fluent Static
        public static EvictingDecoration New(IStore decorated,
            IStore expirableStore,
            LogicOfTo<IHasId, IExpirable> expirableFactory)
        {
            return new EvictingDecoration(decorated, expirableStore, expirableFactory);
        }
        #endregion
    }

    public static class EvictingDecorationExtensions
    {
        /// <summary>
        /// gets the evicting layer
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static EvictingDecoration GetEvicting(this IStore decorated)
        {
            return decorated.As<EvictingDecoration>(true);
        }

        /// <summary>
        /// adds eviction to a store. 
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="evictionConditionStore"></param>
        /// <param name="defaultItemEvictionConditionFactory"></param>
        /// <param name="backgroundIntervalMSecs"></param>
        /// <returns></returns>
        public static EvictingDecoration Evicting(this IStore decorated,
            IStore evictionConditionStore,
            LogicOfTo<IHasId, IExpirable> defaultItemEvictionConditionFactory,
            double backgroundIntervalMSecs = 30000)
        {
            Condition.Requires(decorated).IsNotNull();

            return new EvictingDecoration(decorated, evictionConditionStore, defaultItemEvictionConditionFactory, backgroundIntervalMSecs);
        }

        #region General Eviction
        public static void SaveEvictingItem(this IEvictingStore store, IHasId obj, IExpirable evictingCondition)
        {
            if (store == null)
                return;

            store.Commit(new CommitBag().MarkItemSaved(obj), evictingCondition);
        }
        #endregion

        #region One Time
        public static void SaveLimitedTouchItem(this IEvictingStore store, IHasId obj, int touches)
        {
            if (store == null)
                return;

            var expiry = EvictionPolicy.BuildTouchLimitExpirable(touches);

            store.Commit(new CommitBag().MarkItemSaved(obj), expiry);
        }
        public static void SaveOneTimeItem(this IEvictingStore store, IHasId obj)
        {
            if (store == null)
                return;

            var expiry = EvictionPolicy.BuildTouchLimitExpirable(1);

            store.Commit(new CommitBag().MarkItemSaved(obj), expiry);
        }
        #endregion
    }
}
