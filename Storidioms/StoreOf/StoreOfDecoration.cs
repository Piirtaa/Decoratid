using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Storidioms.ItemValidating;
using System.Collections.Generic;






namespace Decoratid.Storidioms.StoreOf
{
    #region  IStore Of Constructs
    /// <summary>
    /// a store restricted to items that are of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStoreOf<T> : IStore, IValidatingStore where T : IHasId
    {
        new List<T> GetAll();
        List<T> Search(SearchFilterOf<T> filter);
    }
    #endregion

    /// <summary>
    /// Turns a store into a "storeOf".  decorates a store such that the only items that can be stored within the store are of type TItem
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class StoreOfDecoration<T> : ValidatingDecoration, IStoreOf<T> where T : IHasId
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public StoreOfDecoration(IStore decorated)
            : base(decorated, IsOfValidator.New<T>())
        {
        }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            return new StoreOfDecoration<T>(store);
        }
        #endregion

        #region Overrides
        public new List<T> GetAll()
        {
            return base.GetAll().ConvertListTo<T, IHasId>();
        }
        public List<T> Search(SearchFilterOf<T> filter)
        {
            var list = base.Search<T>(filter);
            return list;
        }
        #endregion
    }
}
