using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using System;
using Decoratid.Storidioms.StoreOf;
using Decoratid.Storidioms.Evicting;
using Decoratid.Storidioms;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.Expiring;
using Decoratid.Core.Decorating;
using System.Collections.Generic;
using Decoratid.Storidioms.ItemValidating;
using Decoratid.Core.Decorating;

namespace Decoratid.Idioms.Tasking
{
    /// <summary>
    /// an inmemory task store
    /// </summary>
    public class TaskStore : TaskStoreDecoration
    {
        public TaskStore(LogicOfTo<IHasId, IExpirable> evictionPolicy)
            :base(NaturalInMemoryStore.New(), evictionPolicy)
        {
        }

        public static TaskStore New(LogicOfTo<IHasId, IExpirable> evictionPolicy)
        {
            return new TaskStore(evictionPolicy);
        }
    }

    /// <summary>
    /// decorates a store as a TaskStore so that it is capable of handling the task store responsibilities such as
    /// eviction and validation
    /// </summary>
    public class TaskStoreDecoration : DecoratedStoreBase, ITaskStore
    {
        #region Ctor
        public TaskStoreDecoration(IStore store, LogicOfTo<IHasId, IExpirable> evictionPolicy)
            : base(store.IsOfUniqueId<ITask>().Evicting(NaturalInMemoryStore.New(), evictionPolicy, 1000))
        {
        }
        #endregion

        #region Properties
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            return new TaskStoreDecoration(store, this.ExpirableFactory);
        }
        #endregion

        #region IStoreOfUniqueId<ITask>   
        public ITask GetById(object id)
        {
            var dec = this.FindDecoration<IStoreOfUniqueId<ITask>>(false);
            dec.GetById(id);
            return this.FindDecoration<IStoreOfUniqueId<ITask>>(false).GetById(id);
        }
        public new List<ITask> GetAll()
        {
            return this.FindDecoration<IStoreOfUniqueId<ITask>>(false).GetAll();
        }
        public List<ITask> Search(SearchFilterOf<ITask> filter)
        {
            return this.FindDecoration<IStoreOfUniqueId<ITask>>(false).Search(filter);
        }
        public IItemValidator ItemValidator
        {
            get { return this.FindDecoration<IStoreOfUniqueId<ITask>>(false).ItemValidator; }
        }
        #endregion

        #region IEvictingStore
        public IStore ExpirableStore
        {
            get
            {
                return this.FindDecoration<EvictingDecoration>(true).ExpirableStore;
            }
            set
            {
                this.FindDecoration<EvictingDecoration>(true).ExpirableStore = value;
            }
        }

        public LogicOfTo<IHasId, IExpirable> ExpirableFactory
        {
            get
            {
                return this.FindDecoration<EvictingDecoration>(true).ExpirableFactory;
            }
            set
            {
                this.FindDecoration<EvictingDecoration>(true).ExpirableFactory = value;
            }
        }

        public void Commit(ICommitBag cb, IExpirable expirable)
        {
            this.FindDecoration<EvictingDecoration>(true).Commit(cb, expirable);
        }

        public event EventHandler<EventArgOf<Tuple<IHasId, IExpirable>>> ItemEvicted
        {
            add
            {
                //wire to core registry
                this.FindDecoration<EvictingDecoration>(true).ItemEvicted += value;
            }
            remove
            {
                //wire to core registry
                this.FindDecoration<EvictingDecoration>(true).ItemEvicted -= value;
            }
        }
        public void Evict()
        {
            this.FindDecoration<EvictingDecoration>(true).Evict();
        }
        #endregion
    }


    public static class TaskStoreDecorationExtensions
    {
        /// <summary>
        /// decorates a store as a TaskStore so that it is capable of handling the task store responsibilities such as
        /// eviction and validation
        /// </summary>
        public static ITaskStore MakeTaskStore(this IStore store, LogicOfTo<IHasId, IExpirable> evictionPolicy)
        {
            Condition.Requires(store).IsNotNull();
            Condition.Requires(evictionPolicy).IsNotNull();
            return new TaskStoreDecoration(store, evictionPolicy);
        }

    }
}
