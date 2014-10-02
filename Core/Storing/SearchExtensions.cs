using Decoratid.Core.Identifying;
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
        /// performs a regular Search amongst types T that also have a IHasDependencyOf THasA,and then sorts 
        /// by the dependency from least dependent to most
        /// </summary>
        /// <typeparam name="Tobj"></typeparam>
        /// <typeparam name="THasA"></typeparam>
        /// <param name="store"></param>
        public static List<IHasId> Search_NonGeneric(this ISearchableStore store, Type type, SearchFilter filter)
        {
            if (store == null)
                return null;

            List<IHasId> returnValue = new List<IHasId>();

            //get the type of the 
            var mi = store.GetType().GetMethod("Search");
            MethodInfo generic = mi.MakeGenericMethod(type);
            var retval = generic.Invoke(store, new object[] { filter });

            IEnumerable list = retval as IEnumerable;

            foreach (var each in list)
            {
                returnValue.Add(each as IHasId);
            }

            return returnValue;
        }
    }
}
