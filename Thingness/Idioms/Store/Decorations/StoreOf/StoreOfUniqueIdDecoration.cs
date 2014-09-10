using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Extensions;
using Decoratid.Thingness.Idioms.Store.Decorations.Validating;
using Decoratid.Thingness;
using Decoratid.Thingness.Decorations;

namespace Decoratid.Thingness.Idioms.Store.Decorations.StoreOf
{
    /// <summary>
    /// validates that an IHasId is of T and has a unique id across all other T's
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IsOfUniqueIdValidator<T> : IItemValidator where T : IHasId
    {
        public IsOfUniqueIdValidator(IStore store)
        {
            this.IsValidCondition = new StrategizedConditionOf<IHasId>((x) =>
            {
                if (x == null)
                    return false;

                if (!typeof(T).IsAssignableFrom(x.GetType()))
                    return false;

                //see if the store has an item with the same id and not the same type
                SearchFilterOf<T> filter = new SearchFilterOf<T>((item) =>
                {
                    bool hasSameId = item.Id.Equals(x.Id);
                    bool isSameType = item.GetType().Equals(x.GetType());
                    return hasSameId && (!isSameType);
                });

                var list = store.Search<T>(filter);

                if (list != null && list.Count > 0)
                    return false;

                return true;
            });
        }
        public IStore Store { get; private set; }
        public IConditionOf<IHasId> IsValidCondition { get; private set; }
    }

    #region  IStore Of Constructs
    /// <summary>
    /// Only allows commits of items that are of T. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWriteableStoreOfUniqueId<T> : IValidatingStore where T : IHasId
    {
        ///// <summary>
        ///// hides base/forces implementation of validator to be IsOfUniqueIdValidator
        ///// </summary>
        //new IsOfUniqueIdValidator<T> ItemValidator { get; }
    }


    /// <summary>
    /// a store restricted to items that are of T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStoreOfUniqueId<T> : IStore, IWriteableStoreOfUniqueId<T> where T : IHasId
    {
        T GetById(object id);
    }
    #endregion

    /// <summary>
    /// Is a "store of" and also requires that ids are unique
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StoreOfUniqueIdDecoration<T> : ValidatingDecoration, IStoreOfUniqueId<T> where T : IHasId
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        protected StoreOfUniqueIdDecoration() : base() { }
        public StoreOfUniqueIdDecoration(IStore decorated)
            : base(decorated, new IsOfUniqueIdValidator<T>(decorated))
        {
        }
        #endregion

        //#region Properties
        //public new IsOfUniqueIdValidator<T> ItemValidator
        //{
        //    get { return base.ItemValidator as IsOfUniqueIdValidator<T>; }
        //    set { base.ItemValidator = value; }
        //}
        //#endregion

        #region IStoreOfUniqueId
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
        #endregion
    }
}
