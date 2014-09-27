using Decoratid.Core.Identifying;
using Decoratid.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Decoratid.Core.Storing
{       
    /// <summary>
    /// container of data to "commit"  (eg. save, delete) to the db. 
    /// </summary>
    /// <remarks>wraps a store that contains CommitBagItem</remarks>
    /// 
    [Serializable]
    public sealed class CommitBag : ICommitBag
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        /// <summary>
        /// default ctor.  use this if we're not sharing a commit bag's store
        /// </summary>
        public CommitBag()
        {
            this.ItemsToSave = new List<IHasId>();
            this.ItemsToDelete = new List<StoredObjectId>();
        }
        #endregion

        #region Properties
        public IList<StoredObjectId> ItemsToDelete { get; private set;}
        public IList<IHasId> ItemsToSave {get; private set;}
        #endregion

        #region Calculated Properties
        public int Count { get { lock (this._stateLock) { return this.ItemsToDelete.Count() + this.ItemsToSave.Count(); } } }
        #endregion

        #region Methods
        public ICommitBag MarkItemSaved(IHasId obj)
        {
            if (obj == null) { return this; }

            //we don't allow StoredObjectIds...we have to be dealing with actual objects
            if (obj is StoredObjectId)
                throw new InvalidOperationException("StoredObjectId type is not allowed");

            lock (this._stateLock)
            {
                List<IHasId> list =  this.ItemsToSave as List<IHasId>;
                
                list.RemoveAll(x => StoredObjectId.New(x).Equals(StoredObjectId.New(obj)));

                this.ItemsToSave.Add(obj);
            }
            return this;
        }
        public ICommitBag MarkItemsSaved(List<IHasId> objs)
        {
            objs.WithEach(x =>
            {
                this.MarkItemSaved(x);
            });
            return this;
        }
        /// <summary>
        /// Marks an item for deletion.  Can use IHasId(ie. instance) or StoredObjectId(ie. key) here
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ICommitBag MarkItemDeleted(StoredObjectId key)
        {
            if (key == null) { return this; }

            lock (this._stateLock)
            {
                this.ItemsToDelete.Remove(key);
                this.ItemsToDelete.Add(key);
            }
            return this;
        }
        public ICommitBag MarkItemsDeleted(List<StoredObjectId> objs)
        {
            objs.WithEach(x =>
            {
                this.MarkItemDeleted(x);
            });
            return this;
        }
        #endregion

        #region Static Fluent
        public static CommitBag New()
        {
            return new CommitBag();
        }
        #endregion
    }

}
