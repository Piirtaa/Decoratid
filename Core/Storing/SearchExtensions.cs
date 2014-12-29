using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Decoratid.Core.Storing
{
    public static class SearchExtensions
    {
        /// <summary>
        /// filters out non-T items, and those that don't pass the explicit filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="store"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<T> SearchOf<T>(this ISearchableStore store, LogicOfTo<T, bool> filter) where T : IHasId
        {
            if (store == null)
                return null;

            LogicOfTo<IHasId, bool> filter2 = new LogicOfTo<IHasId, bool>((item) =>
            {
                if (!(item is T))
                    return false;

                T t = (T)item;
                LogicOfTo<T, bool> logic = filter.Perform(t) as LogicOfTo<T, bool>;
                return logic.Result;
            });

            var list = store.Search(filter2);

            return list.ConvertListTo<T, IHasId>();
        }
        /// <summary>
        /// filters out non-type items, and those that don't pass the explicit filter
        /// </summary>
        public static List<IHasId> SearchOf(this ISearchableStore store, Type type, LogicOfTo<IHasId,bool> filter)
        {
            if (store == null)
                return null;

            LogicOfTo<IHasId, bool> filter2 = new LogicOfTo<IHasId, bool>((item) =>
            {
                if(!type.IsAssignableFrom(item.GetType()))
                    return false;

                LogicOfTo<IHasId, bool> logic = filter.Perform(item) as LogicOfTo<IHasId, bool>;
                return logic.Result;
            });

            var list = store.Search(filter2);

            return list;
        }
    }
}
