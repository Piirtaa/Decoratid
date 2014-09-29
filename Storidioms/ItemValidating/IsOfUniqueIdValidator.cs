using CuttingEdge.Conditions;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Storidioms.ItemValidating
{
    /// <summary>
    /// validates that an IHasId is of T and has a unique id across all other T's
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    [Serializable]
    public class IsOfUniqueIdValidator : IItemValidator
    {
        #region Ctor
        public IsOfUniqueIdValidator(Type type, IStore store)
        {
            Condition.Requires(type).IsNotNull();
            this.OfType = type;
            Condition.Requires(store).IsNotNull();
            this.Store = store;

            //build up the condition
            this.IsValidCondition = new StrategizedConditionOf<IHasId>((x) =>
            {
                if (x == null)
                    return false;

                //if not of the same type, skip
                if (!type.IsAssignableFrom(x.GetType()))
                    return false;

                //see if the store has an item with the same id and not the same type
                SearchFilter filter = GetFindSameIdSearchFilter(x);

                var list = store.Search_NonGeneric(type, filter);

                if (list != null && list.Count > 0)
                    return false;

                return true;
            });
        }
        #endregion

        #region Properties
        public Type OfType { get; private set; }
        public IStore Store { get; private set; }
        public IConditionOf<IHasId> IsValidCondition { get; private set; }
        #endregion

        #region Static Helper
        /// <summary>
        /// returns a search filter that finds other objects that have the same id but not the same type
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static SearchFilter GetFindSameIdDiffTypeSearchFilter(IHasId obj)
        {
            SearchFilter filter = SearchFilter.New((item) =>
            {
                bool hasSameId = item.Id.Equals(obj.Id);
                bool isSameType = item.GetType().Equals(obj.GetType());
                return hasSameId && (!isSameType);
            });

            return filter;
        }
        public static SearchFilter GetFindSameIdSearchFilter(IHasId obj)
        {
            SearchFilter filter = SearchFilter.New((item) =>
            {
                bool hasSameId = item.Id.Equals(obj.Id);
                return hasSameId;
            });

            return filter;
        }
        #endregion

        #region Static Methods
        public static IsOfUniqueIdValidator New(Type ofType, IStore store)
        {
            return new IsOfUniqueIdValidator(ofType, store);
        }
        public static IsOfUniqueIdValidator New<T>(IStore store)
        {
            return new IsOfUniqueIdValidator(typeof(T), store);
        }
        #endregion
    }


}
