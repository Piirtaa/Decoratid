using Decoratid.Core;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Idioms.HasBitsing;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using CuttingEdge.Conditions;

namespace Decoratid.Storidioms.Indexing
{
    /*IIndexingStore and IndexingStore contain interface and implementation of a store of SOID->HasBits, essentially.
     * Has an IndexFactory that keeps our index rules and generates HasBits given an IHasId.
     * 
     */ 

    //define type that stores an item's hasbits
    //note: we use the decoration instead of IsA<IHasId<StoredObjectId>, IHasBits> because it implements IHasBits directly and 
    //this is amenable to our parallelization search extensions
    using IndexedEntry = HasBitsIHasIdDecoration; 
    
    public interface IIndexingStore 
    {
        IndexFactory IndexFactory { get; }
        List<StoredObjectId> SearchIndex(Func<IHasBits, bool> filter);
        void SetIndex(IHasId obj);
        IHasBits GetIndex(StoredObjectId id);
        void RemoveIndex(StoredObjectId id);
    }

    [Serializable]
    public class IndexingStore : IIndexingStore
    {
        #region Declarations
        private readonly object _stateLock = new object();
        private IStoreOf<IndexedEntry> _storeOfIndices = NaturalInMemoryStore.New().IsOf<IndexedEntry>();
        #endregion

        #region Ctor
        public IndexingStore()
        {
            this.IndexFactory = new IndexFactory();
        }
        #endregion

        #region Fluent Static
        public static IndexingStore New()
        {
            return new IndexingStore();
        }
        #endregion

        #region IIndexingStore
        public IndexFactory IndexFactory { get; private set; }
        public void SetIndex(IHasId obj)
        {
            Condition.Requires(obj).IsNotNull();

            var hasBits = this.IndexFactory.GenerateIndex(obj);
            var soid = obj.GetStoredObjectId();
            this._storeOfIndices.SaveItem(new IndexedEntry(soid.BuildAsId().HasBits(hasBits)));
        }
        public IHasBits GetIndex(StoredObjectId id)
        {
            var item = this._storeOfIndices.Get<IndexedEntry>(id);
            if (item == null)
                return null;

            return item;
        }
        public void RemoveIndex(StoredObjectId id)
        {
            this._storeOfIndices.DeleteItem(id);
        }
        public List<StoredObjectId> SearchIndex(Func<IHasBits, bool> filter)
        {
            Condition.Requires(filter).IsNotNull();
            var rv = new List<StoredObjectId>();

            var items = this._storeOfIndices.GetAll<IndexedEntry>();
            List<IHasBits> list = new List<IHasBits>();
            list.AddRange(items);

            var filtList = list.FilterList_UsingParallel(filter);
            filtList.WithEach(x =>
            {
                rv.Add((x as IndexedEntry).Id as StoredObjectId);
            });
            return rv;
        }
        #endregion


    }
}
