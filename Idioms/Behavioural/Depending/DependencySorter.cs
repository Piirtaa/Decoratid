using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Dependencies
{
    /// <summary>
    /// Provides fast-algorithm and low-memory usage to sort objects based on their dependencies. 
    /// </summary>
    /// <remarks>
    /// Definition: http://en.wikipedia.org/wiki/Topological_sorting
    /// Source code credited to: http://tawani.blogspot.com/2009/02/topological-sorting-and-cyclic.html    
    /// Original Java source code: http://www.java2s.com/Code/Java/Collections-Data-Structure/Topologicalsorting.htm
    /// </remarks>
    /// <author>ThangTran</author>
    /// <history>
    /// 2012.03.21 - ThangTran: rewritten based on <see cref="TopologicalSorter"/>.
    /// </history>
    public class DependencySorter<T>
    {
        #region Declarations
        private Dictionary<T, List<T>> _matrix = new Dictionary<T, List<T>>();
        #endregion

        #region Ctor
        public DependencySorter()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// call this first to add the objects.  then set dependencies
        /// </summary>
        /// <param name="objects"></param>
        public void AddObjects(T[] objects)
        {
            // add to matrix
            foreach (T obj in objects)
            {
                // add to dictionary
                _matrix.Add(obj, new List<T>());
            }
        }

        /// <summary>
        /// Sets dependencies of given object.
        /// This means <paramref name="obj"/> depends on these <paramref name="dependsOnObjects"/> to run.
        /// Please make sure objects given in the <paramref name="obj"/> and <paramref name="dependsOnObjects"/> are added first.
        /// </summary>
        public void SetDependencies(T obj, T[] dependsOnObjects)
        {
            // for each depended objects, add to dependencies
            foreach (T dependsOnObject in dependsOnObjects)
            {
                _matrix[obj].Add(dependsOnObject);
            }
        }

        /// <summary>
        /// Sorts objects based on this dependencies.
        /// Note: because of the nature of algorithm and memory usage efficiency, this method can be used only one time.
        /// </summary>
        public T[] Sort()
        {
            // prepare result
            List<T> result = new List<T>(_matrix.Count);

            // while there are still object to get
            while (_matrix.Count > 0)
            {
                // get an independent object
                T independentObject;
                if (!this.GetIndependentObject(out independentObject))
                {
                    // circular dependency found
                    throw new CircularReferenceException();
                }

                // add to result
                result.Add(independentObject);

                // delete processed object
                this.DeleteObject(independentObject);
            }

            // return result
            return result.ToArray();
        }

        #endregion


        #region Private methods

        /// <summary>
        /// Returns independent object or returns NULL if no independent object is found.
        /// </summary>
        private bool GetIndependentObject(out T result)
        {
            // for each object
            foreach (KeyValuePair<T, List<T>> pair in _matrix)
            {
                // if the object contains any dependency
                if (pair.Value.Count > 0)
                {
                    // has dependency, skip it
                    continue;
                }

                // found
                result = pair.Key;
                return true;
            }

            // not found
            result = default(T);
            return false;
        }

        /// <summary>
        /// Deletes given object from the matrix.
        /// </summary>
        private void DeleteObject(T obj)
        {
            // delete object from matrix
            _matrix.Remove(obj);

            // for each object, remove the dependency reference
            foreach (KeyValuePair<T, List<T>> pair in _matrix)
            {
                // if current object depends on deleting object
                pair.Value.Remove(obj);
            }
        }


        #endregion
    }

    /// <summary>
    /// Represents a circular reference exception when sorting dependency objects.
    /// </summary>
    public class CircularReferenceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CircularReferenceException"/> class.
        /// </summary>
        public CircularReferenceException()
            : base("Circular reference found.")
        {
        }
    }
}
