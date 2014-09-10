using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness;
using Decoratid.Thingness.Idioms.Logics;
using Decoratid.Thingness.Idioms.ValuesOf;
using Decoratid.Thingness.Decorations;
using Decoratid.Thingness.Idioms.ObjectGraph.Values;
using Decoratid.Thingness.Idioms.ObjectGraph;

namespace Decoratid.Thingness.Idioms.Store.Decorations.Factoried
{
    /// <summary>
    /// a store that can build/save an object from its get, if the get returns nothing
    /// </summary>
    public interface IFactoryStore : IDecoratedStore
    {
        /// <summary>
        /// the strategy used to create an object from an IHasId, when it cannot be found by a Get
        /// </summary>
        LogicOfTo<IStoredObjectId, IHasId> Factory { get; set; }
    }

    /// <summary>
    /// has a factory to build/save an object if it isn't not avail on a get
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class FactoryDecoration : DecoratedStoreBase, IFactoryStore, IHasHydrationMap
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public FactoryDecoration(LogicOfTo<IStoredObjectId, IHasId> factory, IStore decorated)
            : base(decorated)
        {
            Condition.Requires(factory).IsNotNull();
            this.Factory = factory;
        }
        #endregion

        #region IFactoryDecoration
        public LogicOfTo<IStoredObjectId, IHasId> Factory { get; set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new FactoryDecoration(this.Factory, store);

            return returnValue;
        }
        #endregion

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var hydrationMap = new HydrationMapValueManager<FactoryDecoration>();
            hydrationMap.RegisterDefault("Factory", x => x.Factory, (x, y) => { x.Factory = y as LogicOfTo<IStoredObjectId, IHasId>; });
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

        #region Overrides
        /// <summary>
        /// looks for the object but if it isn't there it will be created
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IHasId Get(IStoredObjectId soId)
        {
            var retval = this.Decorated.Get(soId);

            //not here
            if (retval == null)
            {
                //build it
                lock (this._stateLock)
                {
                    retval = this.Factory.CloneAndPerform(soId.ValueOf());

                    //save it
                    if (retval != null)
                    {
                        this.SaveItem(retval);
                    }
                }
            }
            return retval;
        }
        #endregion
    }
}
