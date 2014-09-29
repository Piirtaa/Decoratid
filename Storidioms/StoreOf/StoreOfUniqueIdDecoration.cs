using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Storidioms.ItemValidating;
using System.Collections.Generic;
using System.Linq;
using Decoratid.Extensions;

namespace Decoratid.Storidioms.StoreOf
{
    #region  IStoreOfUniqueId Constructs
    /// <summary>
    /// a store restricted to items that are of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStoreOfUniqueId<T> : IStore, IValidatingStore where T : IHasId
    {
        T GetById(object id);
        new List<T> GetAll();
        List<T> Search(SearchFilterOf<T> filter);
    }
    #endregion

    /// <summary>
    /// Is a "store of" and also requires that ids are unique
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class StoreOfUniqueIdDecoration<T> : ValidatingDecoration, IStoreOfUniqueId<T> where T : IHasId
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public StoreOfUniqueIdDecoration(IStore decorated)
            : base(decorated, IsOfUniqueIdValidator.New<T>(decorated))
        {
        }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            return new StoreOfUniqueIdDecoration<T>(store);
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
        public T GetById(object id)
        {
            SearchFilterOf<T> filter = new SearchFilterOf<T>((item) =>
            {
                bool hasSameId = item.Id.Equals(id);
                return hasSameId;
            });

            var list = this.Search<T>(filter);

            return list.FirstOrDefault();
        }
        #endregion
    }
}
