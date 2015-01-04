using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Storing;
using Decoratid.Core.Logical;
using Decoratid.Core.Identifying;
using Decoratid.Storidioms.StoreOf;
using Decoratid.Core.Decorating;
using Decoratid.Extensions;

namespace Decoratid.Storidioms.Indexing
{
    //type used to contain mapping of index to soids
    using Index2SOIDs = Decoratid.Core.Contextual.ContextualId<string, List<StoredObjectId>>;
    //type used to contain mapping of soid to index
    using SOID2Index = Decoratid.Core.Contextual.ContextualId<StoredObjectId, string>;

    public interface IIndexingStore : IDecoratedStore
    {
        LogicOfTo<IHasId, string> IndexFunction { get; }
        IStoreOfExactly<Index2SOIDs> Index2SOIDStore { get; }
        IStoreOfExactly<SOID2Index> SOID2IndexStore { get; }

        List<IHasId> GetByIndex(string idx);
    }


    [Serializable]
    public class IndexingDecoration : DecoratedStoreBase, IIndexingStore
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public IndexingDecoration(IStore decorated,
            string name,
            LogicOfTo<IHasId, string> indexFunction,
            IStoreOfExactly<Index2SOIDs> index2SOIDStore = null,
            IStoreOfExactly<SOID2Index> sOID2IndexStore = null)
            : base(decorated)
        {
            Condition.Requires(name).IsNotNullOrEmpty();
            this.SetDecorationId(name);

            Condition.Requires(indexFunction).IsNotNull();
            this.IndexFunction = indexFunction;

            if (index2SOIDStore == null)
            {
                this.Index2SOIDStore = NaturalInMemoryStore.New().IsExactlyOf<Index2SOIDs>();
            }
            else
            {
                this.Index2SOIDStore = index2SOIDStore;
            }
            if (sOID2IndexStore == null)
            {
                this.SOID2IndexStore = NaturalInMemoryStore.New().IsExactlyOf<SOID2Index>();
            }
            else
            {
                this.SOID2IndexStore = sOID2IndexStore;
            }
        }
        #endregion

        #region IIndexingStore
        public string Name { get; private set; }
        public LogicOfTo<IHasId, string> IndexFunction { get; private set; }
        public IStoreOfExactly<Index2SOIDs> Index2SOIDStore { get; private set; }
        public IStoreOfExactly<SOID2Index> SOID2IndexStore { get; private set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new IndexingDecoration(store, this.Name, this.IndexFunction, this.Index2SOIDStore, this.SOID2IndexStore);

            return returnValue;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// gets all the items with the provided index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public List<IHasId> GetByIndex(string idx)
        {
            List<IHasId> rv = new List<IHasId>();

            var indexedItem = this.Index2SOIDStore.GetById(idx);
            if (indexedItem != null)
            {
                indexedItem.Context.WithEach(soid =>
                {
                    var item = this.Get(soid);
                    if (item != null)
                        rv.Add(item);
                });
            }

            return rv;
        }

        public override void Commit(ICommitBag bag)
        {
            try
            {
                bag.ItemsToSave.WithEach(x =>
                {
                    this.AddIndex(x);
                });
                bag.ItemsToDelete.WithEach(x =>
                {
                    this.RemoveIndex(x);
                });

                //do actual commit
                this.Decorated.Commit(bag);
            }
            catch
            {
                //reverse
                bag.ItemsToSave.WithEach(x =>
                {
                    this.RemoveIndex(x.GetStoredObjectId());
                });
            }
        }
        #endregion

        #region Helpers
        private void AddIndex(IHasId obj)
        {
            lock (this._stateLock)
            {
                //run the index function to generate an index value
                var logic = this.IndexFunction.Perform(obj) as LogicOfTo<IHasId, string>;
                var idx = logic.Result;

                //get the soid
                var soid = obj.GetStoredObjectId();

                //find the indexed item and update the soid list
                var indexedItem = this.Index2SOIDStore.GetById(idx);
                if (indexedItem != null)
                {
                    if (!indexedItem.Context.Contains(soid))
                        indexedItem.Context.Add(soid);

                    this.Index2SOIDStore.SaveItem(indexedItem);
                }
                else
                {
                    this.Index2SOIDStore.SaveItem(Index2SOIDs.New(idx, soid.AddToList()));
                }
                //update the soid2index 
                this.SOID2IndexStore.SaveItem(SOID2Index.New(soid, idx));
            }
        }
        private void RemoveIndex(StoredObjectId id)
        {
            lock (this._stateLock)
            {
                //get the index by soid lookup
                var soid2Index = this.SOID2IndexStore.GetById(id);
                if (soid2Index != null)
                {
                    //get the indexed item, remove the soid from its list and update it
                    var indexedItem = this.Index2SOIDStore.GetById(soid2Index.Context);
                    if (indexedItem != null)
                    {
                        if (indexedItem.Context.Contains(id))
                            indexedItem.Context.Remove(id);

                        this.Index2SOIDStore.SaveItem(indexedItem);
                    }
                    //remove the soid2index map
                    this.SOID2IndexStore.DeleteItem(id);
                }
            }
        }
        #endregion
    }

    public static class IndexingDecorationExtensions
    {
        /// <summary>
        /// finds the indexing decoration with the specified name
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IndexingDecoration GetIndexing(this IStore decorated, string name)
        {
            return decorated.WalkDecorations((x) =>
            {

                if (x is IndexingDecoration)
                    if (((IndexingDecoration)x).Name.Equals(name))
                        return true;

                return false;
            }) as IndexingDecoration;
        }

        /// <summary>
        /// Adds an indexing decoration
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="indexFunction"></param>
        /// <param name="index2SOIDStore"></param>
        /// <param name="sOID2IndexStore"></param>
        /// <returns></returns>
        public static IndexingDecoration WithIndex(this IStore decorated,
            string name,
            LogicOfTo<IHasId, string> indexFunction,
            IStoreOfExactly<Index2SOIDs> index2SOIDStore = null,
            IStoreOfExactly<SOID2Index> sOID2IndexStore = null)
        {
            Condition.Requires(decorated).IsNotNull();

            return new IndexingDecoration(decorated, name, indexFunction, index2SOIDStore, sOID2IndexStore);
        }

        public static IndexingDecoration WithAlphabeticDecorationIndex(this IStore decorated,
            IStoreOfExactly<Index2SOIDs> index2SOIDStore = null,
            IStoreOfExactly<SOID2Index> sOID2IndexStore = null)
        {
            Condition.Requires(decorated).IsNotNull();

            return new IndexingDecoration(decorated, "AlphabeticDecorationIndex",
                IndexFunctions.AlphabeticDecorationSignatureFunction, index2SOIDStore, sOID2IndexStore);
        }

        public static IndexingDecoration WithExactDecorationIndex(this IStore decorated,
    IStoreOfExactly<Index2SOIDs> index2SOIDStore = null,
    IStoreOfExactly<SOID2Index> sOID2IndexStore = null)
        {
            Condition.Requires(decorated).IsNotNull();

            return new IndexingDecoration(decorated, "ExactDecorationIndex",
                IndexFunctions.ExactDecorationSignatureFunction, index2SOIDStore, sOID2IndexStore);
        }
    }
}
