using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CuttingEdge.Conditions;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Depending.Named
{
    /// <summary>
    /// Decorate a type with this if you want to indicate a dependency of some sort.  
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class NamedDependencyAttribute : Attribute
    {
        public NamedDependencyAttribute(string classification, string self, params string[] deps)
        {
            Condition.Requires(classification).IsNotNullOrEmpty();
            Condition.Requires(self).IsNotNullOrEmpty();

            this.NamedDependency = new NamedDependency();
            this.NamedDependency.Classification = classification;
            this.NamedDependency.Self = self;

            deps.WithEach(x =>
            {
                this.NamedDependency.Prerequisites.Add(x);
            });
        }

        public NamedDependency NamedDependency { get; private set; }


        #region Type Attribute Interrogation
        /// <summary>
        /// helper method.  constructs a NamedDependencyNode instance by examining the type's
        /// NamedDependencyAttributes.                            
        /// </summary>
        /// <param name="type"></param>
        /// <param name="classification"></param>
        /// <returns></returns>
        private static NamedDependency BuildNamedDependencyFromTypeAttributes(Type type, string classification)
        {
            Condition.Requires(type).IsNotNull();
            Condition.Requires(classification).IsNotNullOrEmpty();

            //a dependency is indicated by decoration with NamedDependencyAttribute
            NamedDependency returnValue = null;
            try
            {
                //find the attribute
                var attr = type.GetCustomAttributes<NamedDependencyAttribute>().SingleOrDefault(x => x.NamedDependency.Classification.Equals(classification));
                if (attr != null)
                {
                    returnValue = attr.NamedDependency;
                }
            }
            catch { }
            return returnValue;
        }

        /// <summary>
        /// sorts types that are decorated with DependencyAttribute.  If the type does not have a DependencyAttribute
        /// with the provided classification, it is returned at the beginning (least dependent) of the list with a null dependency.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static List<Tuple<Type, NamedDependency>> GetAttributeAndSort(List<Type> types, string dependencyName)
        {
            //build type map and dep list
            List<Tuple<Type, NamedDependency>> returnValue = new List<Tuple<Type, NamedDependency>>();

            Dictionary<string, Type> depName2Type = new Dictionary<string, Type>();
            Dictionary<Type, NamedDependency> type2dep = new Dictionary<Type, NamedDependency>();
            List<NamedDependency> deps = new List<NamedDependency>();

            types.WithEach(type =>
            {
                //build it
                var dep = BuildNamedDependencyFromTypeAttributes(type, dependencyName);

                //add to type2dep
                type2dep[type] = dep;

                //add to depName2Type if there is a dep
                if (dep != null)
                {
                    deps.Add(dep);

                    //register the type's dependency by self name
                    depName2Type[dep.Self] = type;
                }
            });

            //sort the deps, get the names back
            var stringDeps = NamedDependency.Sort(deps);

            stringDeps.WithEach(depName =>
            {
                //with the dep name, look in the map to get the type
                var type = depName2Type[depName.Self];
                //using the type, get the dep itself
                var dep = type2dep[type];

                returnValue.Add(new Tuple<Type, NamedDependency>(type, dep));
            });

            //now prepend list with types that have no dependency
            var depTypes = depName2Type.Values.ToList();
            var noDepTypes = types.Where(type=> depTypes.Contains(type) == false);

            noDepTypes.WithEach(type =>
            {
                returnValue.Insert(0, new Tuple<Type, NamedDependency>(type, null));
            });
            return returnValue;
        }
        #endregion
    }
}
