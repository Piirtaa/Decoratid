using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness;
using Decoratid.Extensions;
using CuttingEdge.Conditions;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Storing.Core
{
    /// <summary>
    /// simple in-memory store (backed by an underlying Dictionary).  This is the preferred store to use when dealing with in-memory 
    /// storage.
    /// </summary>
    public class NaturalInMemoryStore : IStore, IGetAllableStore
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public NaturalInMemoryStore()
        {
            this.Dictionary = new Dictionary<StoredObjectId, IHasId>();
        }
        #endregion

        #region Properties
        protected Dictionary<StoredObjectId, IHasId> Dictionary { get; set; }
        #endregion

        #region IStore
        public virtual IHasId Get(IStoredObjectId soId)
        {
            IHasId obj = null;

            this.Dictionary.TryGetValue(StoredObjectId.New(soId.ObjectType, soId.ObjectId), out obj);

            return obj;
        }
        public virtual List<T> Search<T>(SearchFilter filter) where T : IHasId
        {
            Condition.Requires(filter).IsNotNull();

            List<T> returnValue = new List<T>();
            Type filterType = typeof(T);

            //lock and retrieve the values
            List<IHasId> vals = new List<IHasId>();
            lock (this._stateLock)
            {
                vals.AddRange(this.Dictionary.Values);
            }

            vals.WithEach(x =>
            {
                //if the item is the wrong type, skip it
                var type = x.GetType();
                if (filterType.IsAssignableFrom(type))
                {
                    if (filter.Filter((T)x))
                    {
                        returnValue.Add((T)x);
                    }
                }
            });

            return returnValue;
        }
        public virtual void Commit(ICommitBag bag)
        {
            Condition.Requires(bag).IsNotNull();

            //lock the store
            lock (this._stateLock)
            {
                bag.ItemsToDelete.WithEach(x =>
                {
                    //we have no filter, assume things are ok
                    this.Dictionary.Remove(x);
                });
                bag.ItemsToSave.WithEach(x =>
                {
                    //we have no filter, assume things are ok
                    this.Dictionary[x.GetStoredObjectId()] = x;
                });
            }
        }
        #endregion

        #region IGetAllable
        public virtual List<IHasId> GetAll()
        {
            return this.Dictionary.Values.ToList();
        }
        #endregion

        #region Static Fluent Methods
        public static NaturalInMemoryStore New()
        {
            return new NaturalInMemoryStore();
        }
        #endregion
    }
}
