using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Extensions;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;

namespace Decoratid.Idioms.Depending
{
    /// <summary>
    /// Has a property, Dependency, that contains the dependency node info (self & deps)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasDependencyOf<T>
    {
        IDependencyOf<T> Dependency { get; }
    }

    /// <summary>
    /// A node (of T), presumably in a graph (of T), that has a list of other nodes (of T) it depends on
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDependencyOf<T>
    {
        T Self { get; }
        List<T> Prerequisites { get; }
    }

    /// <summary>
    /// Container of a single dependency
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DependencyOf<T> : IDependencyOf<T>
    {
        #region Ctor
        public DependencyOf()
        {
            this.Prerequisites = new List<T>();
        }
        public DependencyOf(IDependencyOf<T> node)
        {
            Condition.Requires(node).IsNotNull();
            this.Self = node.Self;
            this.Prerequisites = new List<T>();

            if (node.Prerequisites != null)
                this.Prerequisites.AddRange(node.Prerequisites);
        }
        public DependencyOf(T self, List<T> prerequisites)
        {
            this.Self = self;
            this.Prerequisites = new List<T>();

            if (prerequisites != null)
                this.Prerequisites.AddRange(prerequisites);
        }
        public DependencyOf(T self)
        {
            this.Self = self;
            this.Prerequisites = new List<T>();
        }
        #endregion

        #region Properties
        public T Self { get; set; }
        public List<T> Prerequisites { get; set; }
        #endregion

        #region Static Methods
        /// <summary>
        /// takes a list of dependencies and returns them sorted from least dependent to most
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deps"></param>
        /// <returns></returns>
        public static List<T> Sort(List<DependencyOf<T>> deps)
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
        #endregion
    }
}
