using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Storidioms.ItemValidating;
using System.Collections.Generic;
using Decoratid.Extensions;

namespace Decoratid.Storidioms.StoreOf
{
    #region  IStore Of Exactly Constructs
    /// <summary>
    /// a store of items that are of type T (exact match, no derived types)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStoreOfExactly<T> : IStore, IValidatingStore
        where T : IHasId
    {
        T GetById(object id);
        new List<T> GetAll();
        List<T> Search(SearchFilterOf<T> filter);
    }
    #endregion

    /// <summary>
    /// Turns a store into a "storeOf".  decorates a store such that the only items that can be stored within the store are of type TItem
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class StoreOfExactlyDecoration<T> : ValidatingDecoration, IStoreOfExactly<T> where T : IHasId
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public StoreOfExactlyDecoration(IStore decorated)
            : base(decorated, IsExactlyOfValidator.New<T>())
        {
        }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            return new StoreOfExactlyDecoration<T>(store);
        }
        #endregion

        #region IStoreOfExactly
        public new List<T> GetAll()
        {
            return base.GetAll().ConvertListTo<T, IHasId>();
        }
        public List<T> Search(SearchFilterOf<T> filter)
        {
            var list = base.Search<T>(filter);
            return list;
        }
        public T GetById(object id)
        {
            var item = this.Get(StoredObjectId.New(typeof(T), id));

            if (item == null)
                return default(T);

            return (T)item;
        }
        #endregion
    }
}
