using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Dependencies
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
        public static List<IHasDependencyOf<T>> SortHasADependencies<T>(List<IHasDependencyOf<T>> items)
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
                    map[x.Dependency.Self] = x;
                    deps.Add(x.Dependency);
                }
            });

            returnValue = SortDependenciesAndProjectByLookup<T, IHasDependencyOf<T>>(deps, map);

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
        public static List<Tvalue> SortDependenciesAndProjectByLookup<Tkey,Tvalue>(List<IDependencyOf<Tkey>> deps, Dictionary<Tkey,Tvalue> map)
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
        public static List<T> Sort<T>(List<IDependencyOf<T>> deps)
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
