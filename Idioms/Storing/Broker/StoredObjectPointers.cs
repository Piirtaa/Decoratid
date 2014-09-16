using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

namespace Decoratid.Idioms.Storing.Broker
{
    /// <summary>
    /// a thing that points to a stored object in a store and is able to get/set the object.
    /// Its Id is the same as the stored object id
    /// </summary>
    public interface IStoredObjectPointer : IHasId<StoredObjectId>
    {
        /// <summary>
        /// the id/type of the object being stored
        /// </summary>
        StoredObjectId StoredObjectId { get; }
        /// <summary>
        /// a method to retrieve the object from the store, whereever the store is
        /// </summary>
        /// <returns></returns>
        IHasId GetStoredObject();
        /// <summary>
        /// a method to set the stored object in the store, whereever the store is.  kacks if
        /// the object doesn't have the same storedobjectid as the member StoredObjectId
        /// </summary>
        /// <param name="obj"></param>
        void SetStoredObject(IHasId obj);
    }

    /// <summary>
    /// a reference to a storeditem within a store.  Can think of it as a deferred Get.
    /// </summary>
    public class StoredObjectPointer : IStoredObjectPointer
    {
        #region Ctor
        public StoredObjectPointer(IStore store, StoredObjectId StoredObjectId)
        {
            Condition.Requires(store).IsNotNull();

            this.Store = store;
            this.StoredObjectId = StoredObjectId;
        }
        #endregion

        #region Properties
        public IStore Store { get; protected set; }
        public StoredObjectId StoredObjectId { get; protected set; }
        #endregion

        #region IHasId
        public StoredObjectId Id { get { return this.StoredObjectId; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Methods
        public IHasId GetStoredObject()
        {
            return this.Store.Get(this.StoredObjectId);
        }
        public void SetStoredObject(IHasId obj)
        {
            Condition.Requires(obj).IsNotNull();
            if (!this.StoredObjectId.Equals(new StoredObjectId(obj)))
                throw new ArgumentOutOfRangeException("id mismatch");

            this.Store.SaveItem(obj);
        }
        #endregion


    }

    /// <summary>
    /// a reference to a storeditem within a remote store.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RemoteStoredObjectPointer : IStoredObjectPointer
    {
        #region Ctor
        public RemoteStoredObjectPointer(StoreConnection storeConnection, StoredObjectId StoredObjectId)
        {
            Condition.Requires(storeConnection).IsNotNull();

            this.StoreConnection = storeConnection;
            this.StoredObjectId = StoredObjectId;
        }
        #endregion

        #region Properties
        public StoreConnection StoreConnection { get; protected set; }
        public StoredObjectId StoredObjectId { get; protected set; }
        #endregion

        #region IHasId
        public StoredObjectId Id { get { return this.StoredObjectId; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Methods
        public IHasId GetStoredObject()
        {
            var store = StoreConnectionManager.Instance.GetStore(this.StoreConnection);
            Condition.Requires(store).IsNotNull("Unresolveable Store Connection");

            return store.Get(this.StoredObjectId);
        }
        public void SetStoredObject(IHasId obj)
        {
            Condition.Requires(obj).IsNotNull();

            var store = StoreConnectionManager.Instance.GetStore(this.StoreConnection);
            Condition.Requires(store).IsNotNull("Unresolveable Store Connection");

            if (!this.StoredObjectId.Equals(new StoredObjectId(obj)))
                throw new ArgumentOutOfRangeException("id mismatch");

            store.SaveItem(obj);
        }
        #endregion
    }
}
