using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Storidioms.ItemValidating;
using System.Collections.Generic;
using System.Linq;
using Decoratid.Extensions;
using CuttingEdge.Conditions;
using Decoratid.Core.Logical;

namespace Decoratid.Storidioms.StoreOf
{
    #region  IStoreOfUniqueId Constructs
    /// <summary>
    /// a store restricted to items with unique ids
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStoreOfUniqueId : IStore, IValidatingStore 
    {
        IHasId GetById(object id);
    }
    #endregion

    /// <summary>
    ///  requires that ids are unique
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class StoreOfUniqueIdDecoration : ValidatingDecoration, IStoreOfUniqueId
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public StoreOfUniqueIdDecoration(IStore decorated)
            : base(decorated, UniqueIdValidator.New(decorated))
        {
        }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            return new StoreOfUniqueIdDecoration(store);
        }
        #endregion

        #region Overrides
        public IHasId GetById(object id)
        {
            var filter = UniqueIdValidator.GetFindSameIdSearchFilter(id);
            var list = this.Search(filter);

            return list.FirstOrDefault();
        }
        #endregion
    }

    public static partial class StoreOfUniqueIdDecorationExtensions
    {
        /// <summary>
        /// Is a "store of" and also requires that ids are unique
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static StoreOfUniqueIdDecoration IsOfUniqueId(this IStore decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new StoreOfUniqueIdDecoration(decorated);
            //could alternately do
            //return decorated.WithValidation(new IsOfValidator<T>());
        }
        /// <summary>
        /// replaces an item with the same id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="store"></param>
        /// <param name="item"></param>
        public static void Replace(this IStoreOfUniqueId store, IHasId newItem)
        {
            Condition.Requires(store).IsNotNull();
            if (newItem == null)
                return;

            //get the item with the same id
            var item = store.GetById(newItem.Id);
            store.DeleteItem(item.GetStoredObjectId());
            //commit the new item
            store.SaveItem(newItem);
        }


    }
}
