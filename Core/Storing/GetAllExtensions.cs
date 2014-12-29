using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Decoratid.Core.Storing
{
    public static class GetAllExtensions
    {
        /// <summary>
        /// does a search that finds items that are of compatible types and have same id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="store"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<T> GetAllById<T>(this ISearchableStore store, object id) where T : IHasId
        {
            if (store == null)
                return null;

            LogicOfTo<IHasId,bool> filter = new LogicOfTo<IHasId,bool>((item) =>
            {
                if (!(item is T))
                    return false;

                return item.Id.Equals(id);
            });

            var list = store.Search(filter);

            return list.ConvertListTo<T,IHasId>();
        }

        public static List<T> GetAll<T>(this ISearchableStore store) where T : IHasId
        {
            if (store == null)
                return null;

            LogicOfTo<IHasId, bool> filter = new LogicOfTo<IHasId, bool>((item) =>
            {
                if (!(item is T))
                    return false;

                return true;
            });

            var list = store.Search(filter);

            return list.ConvertListTo<T, IHasId>();
        }
 
    }
}
