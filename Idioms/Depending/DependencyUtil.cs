using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Depending
{
    /// <summary>
    /// utility helper class for using dependencies
    /// </summary>
    public static class DependencyUtil
    {
        /// <summary>
        /// If an object aggregates (rather than inherits) a dependency, we want to sort on the dependency
        /// but return the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static List<IHasDependencyOf<T>> SortHasADependencies<T>(this List<IHasDependencyOf<T>> items)
        {
            List<IHasDependencyOf<T>> returnValue = new List<IHasDependencyOf<T>>();

            //build a dictionary of the HasA, keyed by T, the dependency type
            //build a list of the dependency containers
            Dictionary<T, IHasDependencyOf<T>> map = new Dictionary<T, IHasDependencyOf<T>>();
            List<IDependencyOf<T>> deps = new List<IDependencyOf<T>>();
            items.WithEach(x =>
            {
                if (x.Dependency != null)
                {
                    map[x.Dependency.Self] = x; //key each item by the Self dependency value
                    deps.Add(x.Dependency); //aggregate list of the dependencies themselves (which we will later sort and then pull values from using the keyed map above)
                }
            });

            returnValue = SortDependenciesAndLookupValues<T, IHasDependencyOf<T>>(deps, map);

            return returnValue;
        }
        /// <summary>
        /// using the dependency as the key in a key/value store, sorts the dependencies (keys) and returns the values
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="deps"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static List<Tvalue> SortDependenciesAndLookupValues<Tkey,Tvalue>(this List<IDependencyOf<Tkey>> deps, Dictionary<Tkey,Tvalue> map)
        {
            List<Tvalue> returnValue = new List<Tvalue>();

            var orderedList = Sort<Tkey>(deps);

            orderedList.WithEach(x =>
            {
                if (map.ContainsKey(x))
                {
                    returnValue.Add(map[x]);
                }
            });

            return returnValue;
        }
        /// <summary>
        /// takes a list of dependencies and returns them sorted from least dependent to most
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deps"></param>
        /// <returns></returns>
        public static List<T> Sort<T>(this List<IDependencyOf<T>> deps)
        {
            List<T> returnValue = new List<T>();

            DependencySorter<T> sorter = new DependencySorter<T>();

            List<T> items = new List<T>();
            deps.WithEach(dep =>
            {
                items.Add(dep.Self);
            });
            sorter.AddObjects(items.ToArray());

            deps.WithEach(dep =>
            {
                sorter.SetDependencies(dep.Self, dep.Prerequisites.ToArray());
            });

            returnValue = sorter.Sort().ToList();

            return returnValue;
        }
    }
}
