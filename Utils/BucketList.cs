using CuttingEdge.Conditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Utils
{
    /// <summary>
    /// sits on top of a list, and maintains an enumerable bucketing of the provided list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBucketList<T> : IEnumerable<IEnumerable<T>>
    {
        IEnumerable<T> Decorated { get; }
        int BucketCount { get; }

    }
    /// <summary>
    /// a list that has bucket semantics on top of just being a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BucketList<T> : IEnumerable<IEnumerable<T>>
    {
        #region Declarations
        private int _initialBucketSize;
        private List<Tuple<int, object>> _buckets = new List<Tuple<int, object>>(); //holds starting index of each bucket
        private readonly IEnumerable<T> _decorated;
        #endregion

        #region Ctor
        public BucketList(IEnumerable<T> decorated, int initialBucketSize)
        {
            Condition.Requires(decorated).IsNotNull();
            this._decorated = decorated;
            this._initialBucketSize = initialBucketSize;

            
        }
        #endregion

        #region Properties
        public IEnumerable<T> Decorated { get { return _decorated; } }
        public int Count
        {
            get { return this._buckets.Count; }
        }
        public IEnumerable<T> this[int i]
        {
            get
            {

            }
        }
        #endregion



        #region IEnumerable
        public IEnumerator<T> GetEnumerator()
        {
            List<T> rv = new List<T>();

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
