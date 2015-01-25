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
using Decoratid.Idioms.HasBitsing;

namespace Decoratid.Storidioms.Indexing
{
    
    public interface IIndexingStoreDecoration : IDecoratedStore
    {
        IIndexingStore StoreOfIndices { get; }
        List<IHasId> SearchIndex(Func<IHasBits, bool> filter);
    }

    [Serializable]
    public class IndexingDecoration : DecoratedStoreBase, IIndexingStoreDecoration
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public IndexingDecoration(IStore decorated)
            : base(decorated)
        {
            this.StoreOfIndices = IndexingStore.New();
        }
        #endregion

        #region IIndexingStoreDecoration
        public IIndexingStore StoreOfIndices { get; private set; }
        public List<IHasId> SearchIndex(Func<IHasBits, bool> filter)
        {
            var rv = new List<IHasId>();

            var soids = this.StoreOfIndices.SearchIndex(filter);

            soids.WithEach(x =>
            {
                var item = this.Get(x);
                if (item != null)
                    rv.Add(item);
            });

            return rv;
        }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new IndexingDecoration(store);

            return returnValue;
        }
        #endregion

        #region Overrides
        public override void Commit(ICommitBag bag)
        {
            try
            {
                bag.ItemsToSave.WithEach(x =>
                {
                    this.StoreOfIndices.SetIndex(x);
                });
                bag.ItemsToDelete.WithEach(x =>
                {
                    this.StoreOfIndices.RemoveIndex(x);
                });

                //do actual commit
                this.Decorated.Commit(bag);
            }
            catch
            {
                //reverse
                bag.ItemsToSave.WithEach(x =>
                {
                    this.StoreOfIndices.RemoveIndex(x.GetStoredObjectId());
                });
            }
        }
        #endregion

    }

    public static class IndexingDecorationExtensions
    {

        /// <summary>
        /// Adds an indexing decoration
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="indexFunction"></param>
        /// <param name="index2SOIDStore"></param>
        /// <param name="sOID2IndexStore"></param>
        /// <returns></returns>
        public static IndexingDecoration WithIndex(this IStore decorated)
        {
            Condition.Requires(decorated).IsNotNull();

            return new IndexingDecoration(decorated);
        }

    }
}
