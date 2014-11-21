using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Decoratid.Core.Storing
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
            IHasId rv = null;

#if DEBUG
            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store getting {3}", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, soId.With(x => x.ToString())));
#endif

            this.Dictionary.TryGetValue(StoredObjectId.New(soId.ObjectType, soId.ObjectId), out rv);


#if DEBUG
            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store get returns {3}", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, rv.With(x => x.GetStoredObjectId().ToString())));
#endif

            return rv;
        }
        public virtual List<T> Search<T>(SearchFilter filter) where T : IHasId
        {
            Condition.Requires(filter).IsNotNull();

            List<T> returnValue = new List<T>();
            Type filterType = typeof(T);

#if DEBUG
            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store search starts", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName));
#endif

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

#if DEBUG
            returnValue.WithEach(x =>
            {
                Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store search returns {3}", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, x.GetStoredObjectId().ToString()));
            });
            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store search ends", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName));

#endif

            return returnValue;
        }
        public virtual void Commit(ICommitBag bag)
        {
            Condition.Requires(bag).IsNotNull();

#if DEBUG
            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store commit starts", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName));

            bag.ItemsToSave.WithEach(x =>
            {
                Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store saving {3}", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, x.GetStoredObjectId().ToString()));
            });
            bag.ItemsToDelete.WithEach(x =>
            {
                Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store deleting {3}", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, x.ToString()));
            });
#endif

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

#if DEBUG
            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store commit ends", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName));
#endif
        }
        #endregion

        #region IGetAllable
        public virtual List<IHasId> GetAll()
        {
#if DEBUG
            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store get all starts", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName));
#endif
            var rv = this.Dictionary.Values.ToList();
#if DEBUG
            rv.WithEach(x =>
            {
                Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store get all returns {3}", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, x.GetStoredObjectId().ToString()));
            });
            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store get all ends", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName));

#endif
            return rv;
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
