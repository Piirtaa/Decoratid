using CuttingEdge.Conditions;
using Decoratid.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decoratid.Idioms.Depending
{

    /// <summary>
    /// Container of a single dependency
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// 
    [Serializable]
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
        public T Self { get; private set; }
        public List<T> Prerequisites { get; private set; }
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
