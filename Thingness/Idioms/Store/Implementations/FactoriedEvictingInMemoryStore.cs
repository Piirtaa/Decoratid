using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness;
using Decoratid.Extensions;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Store.Decorations;
using Decoratid.Thingness.Idioms.Store.CoreStores;
using Decoratid.Thingness.Idioms.Store.Decorations.Factoried;
using Decoratid.Thingness.Idioms.Store.Decorations.Evicting;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Thingness.Idioms.Logics;
using Decoratid.Thingness.Decorations;
using Decoratid.Thingness.Idioms.ObjectGraph;

namespace Decoratid.Thingness.Idioms.Store.Implementations
{
    /// <summary>
    /// A simple in-memory store with events and eviction, that builds items ifthey aren't found.
    /// Typically use in a short term cache type of situation where we know how to build things.
    /// </summary>
    public class FactoriedEvictingInMemoryStore : DecoratedStoreBase,  IEvictingStore, IFactoryStore
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public FactoriedEvictingInMemoryStore(LogicOfTo<IStoredObjectId, IHasId> factory, 
            LogicOfTo<IHasId, ICondition> defaultItemEvictionConditionFactory, 
            double backgroundIntervalMSecs = 30000)
            : base(InMemoryStore.New()
            .DecorateWithEviction(InMemoryStore.New(), 
            defaultItemEvictionConditionFactory, backgroundIntervalMSecs)
            .DecorateWithFactory(factory))
        {
        }
        #endregion


        #region IDecoratedStore
        /// <summary>
        /// throw not implemented exception on "decorated in-memory stores"
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            throw new NotImplementedException();
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

        public LogicOfTo<IHasId, Conditions.ICondition> DefaultItemEvictionConditionFactory
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

        public void Commit(ICommitBag cb, Conditions.ICondition evictionCondition)
        {
            this.FindDecoratorOf<EvictDecoration>(true).Commit(cb, evictionCondition);
        }

        public event EventHandler<EventArgOf<Tuple<IHasId, Conditions.ICondition>>> ItemEvicted
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

        #region IFactoryStore
        public LogicOfTo<IStoredObjectId, IHasId> Factory
        {
            get
            {
                return this.FindDecoratorOf<FactoryDecoration>(true).Factory;
            }
            set
            {
                this.FindDecoratorOf<FactoryDecoration>(true).Factory = value;
            }
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
    }
}
