using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Storidiom.Thingness;
using Storidiom.Thingness.Idioms.Logics;
using Storidiom.Thingness.Idioms.ValuesOf;

namespace Storidiom.Thingness.Idioms.Store.Decorations.Indexing
{
    /// <summary>
    /// a store that keeps an index of items (by StoredObjectId) to facilitate quick lookup
    /// </summary>
    public interface IIndexedStore : IDecoratedStore
    {
        
    }

    /// <summary>
    /// has a factory to build/save an object if it isn't not avail on a get
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// 
    [Serializable]
    public class FactoryDecoration : DecoratedStoreBase, IFactoryStore
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
