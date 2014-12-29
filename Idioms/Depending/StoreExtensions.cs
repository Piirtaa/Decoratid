using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Idioms.Depending;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Decoratid.Idioms.Depending
{
    public static class StoreExtensions
    {
        /// <summary>
        /// performs a regular Search amongst types T that also have a IHasDependencyOf THasA,and then sorts 
        /// by the dependency from least dependent to most
        /// </summary>
        /// <typeparam name="Tobj"></typeparam>
        /// <typeparam name="THasA"></typeparam>
        /// <param name="store"></param>
        public static List<T> SearchHasADependency<T, THasA>(this ISearchableStore store, LogicOfTo<T, bool> filter)
    where T : IHasId, IHasDependencyOf<THasA>
        {
            if (store == null)
                return null;

            var list = store.SearchOf<T>(filter);

            List<IHasDependencyOf<THasA>> depList = new List<IHasDependencyOf<THasA>>();
            list.WithEach(x =>
            {
                depList.Add(x);
            });

            //sort deplist
            depList = DependencyUtil.SortHasADependencies(depList);


            //convert to T
            List<T> returnValue = new List<T>();

            depList.WithEach(x =>
            {
                returnValue.Add((T)x);
            });

            return returnValue;
        }
        /// <summary>
        /// performs a regular GetAll of T, where T is IHasDependencyOf THasA, sorted by the Dependency
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="THasA"></typeparam>
        /// <param name="store"></param>
        public static List<T> GetAllHasADependency<T, THasA>(this ISearchableStore store)
            where T : IHasId, IHasDependencyOf<THasA>
        {
            if (store == null)
                return null;

            var list = store.GetAll<T>();

            List<IHasDependencyOf<THasA>> depList = new List<IHasDependencyOf<THasA>>();
            list.WithEach(x =>
            {
                depList.Add(x);
            });

            //sort deplist
            depList = DependencyUtil.SortHasADependencies(depList);


            //convert to T
            List<T> returnValue = new List<T>();

            depList.WithEach(x =>
            {
                returnValue.Add((T)x);
            });

            return returnValue;
        }
    }
}
