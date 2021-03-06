﻿using CuttingEdge.Conditions;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Storidioms.ItemValidating
{
    /// <summary>
    /// validates that the item has a unique id within the store
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class UniqueIdValidator : IItemValidator
    {
        #region Ctor
        public UniqueIdValidator(IStore store)
        {
            Condition.Requires(store).IsNotNull();
            this.Store = store;

            //build up the condition
            this.IsValidCondition = new StrategizedConditionOf<IHasId>((x) =>
            {
                if (x == null)
                    return false;

                //see if the store has an item with the same id and not the same type
                var filter = GetFindSameIdDiffTypeSearchFilter(x);

                var list = store.Search(filter);

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
        public static LogicOfTo<IHasId,bool> GetFindSameIdDiffTypeSearchFilter(IHasId obj)
        {
            LogicOfTo<IHasId, bool> filter = LogicOfTo<IHasId, bool>.New((item) =>
            {
                bool hasSameId = item.Id.Equals(obj.Id);
                bool isSameType = item.GetType().Equals(obj.GetType());
                return hasSameId && (!isSameType);
            });

            return filter;
        }
        public static LogicOfTo<IHasId, bool> GetFindSameIdSearchFilter(object id)
        {
            LogicOfTo<IHasId, bool> filter = LogicOfTo<IHasId, bool>.New((item) =>
            {
                bool hasSameId = item.Id.Equals(id);
                return hasSameId;
            });

            return filter;
        }
        #endregion

        #region Static Methods
        public static UniqueIdValidator New(IStore store)
        {
            return new UniqueIdValidator(store);
        }
        #endregion
    }


}
