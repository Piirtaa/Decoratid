using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Storing;
using Decoratid.Core.Storing.Decorations.Polling;
using Decoratid.Core.Storing.Decorations.StoreOf;
using Decoratid.Thingness;
using Decoratid.Extensions;
using Decoratid.Core.Storing.Decorations.Evicting;
using Decoratid.Core.Conditional;
using Decoratid.Core.Conditional.Common;
using Decoratid.Tasks.Core;
using Decoratid.Tasks.Decorations;
using Decoratid.Core.Storing;
using Decoratid.Core.Storing.Decorations;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing.Decorations.Validating;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Tasks
{
    /// <summary>
    /// decorates a store as a TaskStore so that it is capable of handling the task store responsibilities such as
    /// eviction and validation
    /// </summary>
    public class TaskStoreDecoration : DecoratedStoreBase, ITaskStore
    {
        #region Ctor
        public TaskStoreDecoration(IStore store, LogicOfTo<IHasId, ICondition> evictionPolicy)
            : base(store.DecorateWithIsOfUniqueId<ITask>().DecorateWithEviction(InMemoryStore.New(), evictionPolicy, 1000))
        {
        }
        #endregion

        #region Properties
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            return new TaskStoreDecoration(store, this.DefaultItemEvictionConditionFactory);
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return string.Empty;
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {
        }
        #endregion

        #region IStoreOfUniqueId<ITask>   
        public ITask GetById(object id)
        {
            return this.FindDecoratorOf<IStoreOfUniqueId<ITask>>(false).GetById(id);
        }
        public IsOfUniqueIdValidator<ITask> ItemValidator
        {
            get { return this.FindDecoratorOf<IStoreOfUniqueId<ITask>>(false).ItemValidator as IsOfUniqueIdValidator<ITask>; }
        }
        IItemValidator IValidatingStore.ItemValidator
        {
                get { return this.ItemValidator; }
        }
        #endregion

        #region IEvictingStore
        public IStore EvictionConditionStore
        {
            get
            {
                return this.FindDecoratorOf<EvictDecoration>(true).EvictionConditionStore;
            }
            set
            {
                this.FindDecoratorOf<EvictDecoration>(true).EvictionConditionStore = value;
            }
        }

        public LogicOfTo<IHasId, ICondition> DefaultItemEvictionConditionFactory
        {
            get
            {
                return this.FindDecoratorOf<EvictDecoration>(true).DefaultItemEvictionConditionFactory;
            }
            set
            {
                this.FindDecoratorOf<EvictDecoration>(true).DefaultItemEvictionConditionFactory = value;
            }
        }

        public void Commit(ICommitBag cb, ICondition evictionCondition)
        {
            this.FindDecoratorOf<EvictDecoration>(true).Commit(cb, evictionCondition);
        }

        public event EventHandler<EventArgOf<Tuple<IHasId, ICondition>>> ItemEvicted
        {
            add
            {
                //wire to core registry
                this.FindDecoratorOf<EvictDecoration>(true).ItemEvicted += value;
            }
            remove
            {
                //wire to core registry
                this.FindDecoratorOf<EvictDecoration>(true).ItemEvicted -= value;
            }
        }
        public void Evict()
        {
            this.FindDecoratorOf<EvictDecoration>(true).Evict();
        }
        #endregion

    }


    public static partial class Extensions
    {
        /// <summary>
        /// decorates a store as a TaskStore so that it is capable of handling the task store responsibilities such as
        /// eviction and validation
        /// </summary>
        public static ITaskStore DecorateAsTaskStore(this IStore store, LogicOfTo<IHasId, ICondition> evictionPolicy)
        {
            Condition.Requires(store).IsNotNull();
            Condition.Requires(evictionPolicy).IsNotNull();
            return new TaskStoreDecoration(store, evictionPolicy);
        }

    }
}
