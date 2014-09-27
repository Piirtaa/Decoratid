using Decoratid.Core.Contextual;
using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;

namespace Decoratid.Core.Storing
{
    /// <summary>
    /// A bitwise enum of Store Operations
    /// </summary>
    [Flags]
    public enum StoreOperation
    {
        Get = 1,
        Search = 2,
        Commit = 4,
        GetAll = 8
    }

    /// <summary>
    /// the different ways (modes) a stored object can be accessed
    /// </summary>
    [Flags]
    public enum StoredItemAccessMode
    {
        Read = 1,
        Save = 2,
        Delete = 4
    }

    /// <summary>
    /// defines a key to a stored object.  
    /// </summary>
    public interface IStoredObjectId : IHasId<Tuple<Type, object>>
    {
        Type ObjectType { get; }
        object ObjectId { get; }
    }


    #region IStore constructs
    /// <summary>
    /// a store that provides single item lookup via a type and an IHasId.  It is not covariant/contravariant.  The item 
    /// must correspond exactly to the type and Id.
    /// </summary>
    public interface IGettableStore
    {
        /// <summary>
        /// returns the exact item specified (same exact type, same id).  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        IHasId Get(IStoredObjectId soId);
    }

    /// <summary>
    /// a store that provides search for a given type via a search criteria argument (ie. SearchFilter, essentially Func IHasId,bool)
    /// </summary>
    public interface ISearchableStore
    {
        /// <summary>
        /// searches for a matching item (same generic type).  Does an IsA test
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        List<T> Search<T>(SearchFilter filter) where T : IHasId;
    }
    /// <summary>
    /// a store that provides the ability to retrieve the entire store's contents 
    /// </summary>
    public interface IGetAllableStore
    {
        List<IHasId> GetAll();
    }

    /// <summary>
    /// the fundamentals of a commit - things to save, things to remove
    /// </summary>
    /// <remarks>
    /// Sadly we do not fractalize this into a store - we have reached the seed of the concept. 
    /// </remarks>
    public interface ICommitBag
    {
        IList<StoredObjectId> ItemsToDelete { get; }
        IList<IHasId> ItemsToSave { get; }
        ICommitBag MarkItemsSaved(List<IHasId> objs);
        ICommitBag MarkItemsDeleted(List<StoredObjectId> objs);
    }

    /// <summary>
    /// a store that provides the ability to store and remove things.
    /// </summary>
    public interface IWriteableStore
    {
        /// <summary>
        /// saves/deletes a bunch of IHasIds
        /// </summary>
        /// <param name="bag"></param>
        void Commit(ICommitBag bag);
    }

    /// <summary>
    /// Defines the core responsibilities of a store of IHasId.  Getting, Saving, Removing, Searching, Getting All.
    /// </summary>
    public interface IStore : ISearchableStore, IWriteableStore, IGettableStore, IGetAllableStore
    {
    }

    #endregion


    #region Misc
    public interface IHasDateCreated
    {
        DateTime DateCreated { get; }
    }


    public interface IMutable : IHasId
    {
        void Mutate();
    }
    #endregion


}
