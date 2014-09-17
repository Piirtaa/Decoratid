using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Dependencies.Named
{
    /*
    NamedDependency is used to model the dependency between things, using names/strings as a proxy to the actual thing.

    A named dependency is a dependencyof string with an additional property, Classification, to assist with grouping.
    
    The design process:
	    Given the following requirements:
	    -able to apply dependency to a Type with attributes
	    -able to apply dependency to an Instance with interfaces
	    -classification of dependencies
     *          -for grouping 
     *          -so a Type or Instance can have multiple, differing classes of a dependencies

	    and constraints:
	    -Attributes support a limited set of property types (eg. primitives and strings)

	    We produce:
	    NamedDependency
		    - is fundamentally a "dependency of string" with an additional Classification property.  (Req 3)
		    - is amenable to conversion to attributes because it uses strings (Req 1)
     */


    /// <summary>
    /// A dependency node of strings, with an additional classification property. 
    /// </summary>
    /// <remarks>   
    /// Why even have this interface/and its concrete pair?  Because it's the most simple case
    /// one can have in terms of modelling something with multiple types of dependencies.  Rather than real
    /// instances that depend on one another, we use strings as proxies.  And the Classification property 
    /// gives us the ability to have distinguishable multiple instances of a dependency on the same thing.
    /// </remarks>
    public interface INamedDependency : IDependencyOf<string>
    {
        string Classification { get; }
    }

    /// <summary>
    /// Concrete implementation of INamedDependency
    /// </summary>
    [Serializable]
    public class NamedDependency : DependencyOf<string>, INamedDependency
    {
        #region Ctor
        public NamedDependency() : base()
        {
        }
        #endregion

        #region Properties
        public string Classification
        {
            get ; set; 
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// with a list of dependencies and a dependency,return all it depends on, sorted from least dependent to most.
        /// Note: it just returns the exact dependency tree for the rootDep, and does not include any dependencies that
        /// are less dependant within the provided set.
        /// </summary>
        /// <param name="alldeps"></param>
        /// <param name="rootDep"></param>
        /// <returns></returns>
        public static List<NamedDependency> GetSortedDependencies(List<NamedDependency> alldeps, NamedDependency rootDep)
        {
            //if we have no dependencies, carry on
            if(rootDep.Prerequisites==null || rootDep.Prerequisites.Count == 0){return null;}

            //define strategy to find dependencies with the provided names
            Func<List<string>, List<NamedDependency>> lookupDeps = (depNames) =>
            {
                return alldeps.Filter((nd) =>
                {
                    return depNames.Contains(nd.Self);
                });
            };

            //walk the dependency starting at the root dependency
            List<NamedDependency> depTree = new List<NamedDependency>(); //tree to build
            List<NamedDependency> iterDeps = lookupDeps(rootDep.Prerequisites);//temp iterator
            while (iterDeps != null && iterDeps.Count > 0)
            {
                List<string> nextDeps = new List<string>();

                iterDeps.WithEach(dep =>
                {
                    //add to tree if it hasn't happened
                    if (depTree.Exists(x=>x.Self == dep.Self))
                    {
                        depTree.Add(dep);
                    
                        //aggregate the next deps to iterate on
                        nextDeps.AddRange(dep.Prerequisites);
                    }
                });

                //iterate
                iterDeps = lookupDeps(nextDeps);
            }

            //now sort this limited set using the regular sort routine
            var sortedDeps = Sort(depTree);

            //build the return list
            List<NamedDependency> returnValue = new List<NamedDependency>();
            sortedDeps.WithEach(key =>
            {
                var depInstance = depTree.SingleOrDefault(x => x.Self == key.Self);
                if (depInstance != null)
                {
                    returnValue.Add(depInstance);
                }
            });
            return returnValue;
        }
       
        /// <summary>
        /// takes a list of dependencies and returns their names sorted from least dependent to most
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deps"></param>
        /// <returns></returns>
        public static List<NamedDependency> Sort(List<NamedDependency> deps)
        {
            if (deps == null) { return null; }
            
            List<DependencyOf<string>> items = new List<DependencyOf<string>>();
            items.AddRange(deps);
            var sortedStrings = DependencyOf<string>.Sort(items);

            List<NamedDependency> returnValue = new List<NamedDependency>();
            sortedStrings.WithEach(x =>
            {
                returnValue.Add(deps.SingleOrDefault(y => y.Self == x));
            });
            return returnValue;
        }

        /// <summary>
        /// using dependency name as the key in hash of T, sort and produce T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deps"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static List<T> Sort<T>(List<NamedDependency> deps, Dictionary<string, T> map)
        {
            
            List<T> returnValue = new List<T>();
            var list = Sort(deps);
            list.WithEach(x =>
            {
                if (map.ContainsKey(x.Self))
                {
                    returnValue.Add(map[x.Self]);
                }
            });
            return returnValue;
        }


        #endregion
    }
}
