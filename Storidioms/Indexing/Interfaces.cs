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

namespace Decoratid.Storidioms.Indexing
{
       
    /* What is Indexing?
     * Indexing provides additional high-performance bitwise query methods on a store's items. 
     * It is implemented as store of HasBits keyed by item Id.  Queries on this store will
     * point to an SOID that the store contains.  Updates to an item will regenerate the item's 
     * indexed entry.
     * 
     * 
     */

    /// <summary>
    /// the index of for a stored thing
    /// </summary>
    public interface IIndexedEntry : IHasId<StoredObjectId>, IHasBits
    {
    }

    public interface IIndexingStore : IDecoratedStore
    {
        IStoreOf<IHasBitsHasId> StoreOfIndices { get; }
        IIndexGenerator IndexGenerator { get; }
        List<IHasId> SearchIndex(Func<IHasBits, bool> filter);
    }

    public interface IIndexGenerator
    {
        void AddBitToIndex(string name, Func<IHasId, bool> HasBitLogic, int index = -1);
        List<IIndexingBitLogic> BitLogic { get; }
        IHasBits GenerateIndex(IHasId obj);
    }

    /// <summary>
    /// the logic that says whether a thing has a particular bit flag
    /// </summary>
    public interface IIndexingBitLogic
    {
        string Name { get; }
        int BitIndex { get; }
        /// <summary>
        /// logic that says whether a thing has this particular bit
        /// </summary>
        LogicOfTo<IHasId, bool> HasIndexFunction { get; }
    }


}
