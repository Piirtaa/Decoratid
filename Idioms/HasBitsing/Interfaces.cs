using CuttingEdge.Conditions;
using Decoratid.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Decoratid.Idioms.HasBitsing
{
    /// <summary>
    /// something that is convertible to a bit array.  something that has bits.  
    /// common denominator on diff bit tweaking approaches (BitVector32,Array of bool, BitArray, etc)
    /// </summary>
    public interface IHasBits
    {
        BitArray Bits { get; }
        int BitCount { get; }
        void SetBit(int i, bool val);
        /// <summary>
        /// must return NULL when the item doesn't exist in the array/bad index
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        bool? GetBit(int i);
        IHasBits AND(IHasBits bits);
    }


    public static class IHasBitsExtensions
    {
        /*
         * parallelized item task:  Func<IHasBits, bool> is filter function.  we accumulate that which passes
         * 
         * 
         */
        #region Custom Parallelized IHasBits handling
        private class HasBitsFilterWorker
        {
            //private readonly object _stateLock = new object();
            public Thread Thread;
            public List<IHasBits> Accumulator = new List<IHasBits>();
            public int Start, End;
            public List<IHasBits> SourceList = new List<IHasBits>();
            public Func<IHasBits, bool> Filter;

            public HasBitsFilterWorker(int start, int end, List<IHasBits> sourceList, Func<IHasBits, bool> filter)
            {
                this.Start = start;
                this.End = end;
                this.SourceList = sourceList;
                this.Filter = filter;

                this.Thread = new Thread(Func);
                this.Thread.Start();
            }
            public void Func()
            {
                for (int i = Start; i < End; i++)
                {
                    IHasBits item = this.SourceList[i];
                    if (Filter(item))
                    {
                        //lock (this._stateLock)
                        //{
                            Accumulator.Add(item);
                        //}
                    }
                }
            }
        }

        public static List<IHasBits> FilterList_UsingWorkers(this List<IHasBits> sourceList, Func<IHasBits, bool> filter, int numThreads)
        {
            List<IHasBits> rv = new List<IHasBits>();

            var slices = NumbersUtil.GetSliceBounds(0, sourceList.Count,  numThreads > sourceList.Count ? sourceList.Count : numThreads );
            var workers = new HasBitsFilterWorker[slices.Length];
            for (int i = 0; i < slices.Length; i++)
                workers[i] = new HasBitsFilterWorker(slices[i][0], slices[i][1], sourceList, filter);

            foreach (var w in workers)
                w.Thread.Join();

            for (int i = 0; i < workers.Length; i++)
                rv.AddRange(workers[i].Accumulator);

            return rv;
        }
        #endregion

        public static List<IHasBits> FilterList_UsingParallel(this List<IHasBits> sourceList, Func<IHasBits, bool> filter)
        {
            List<IHasBits> rv = new List<IHasBits>();

            if (sourceList == null || sourceList.Count == 0)
                return rv;

            int count = sourceList.Count;
            Parallel.For(0, count, () => new List<IHasBits>(),
                (x, loop, subList) =>
                {
                    var item = sourceList[x];
                    if (filter(item))
                        subList.Add(item);
                    return subList;
                },
                (x) => { rv.AddRange(x); }
            );
            return rv;
        }

        public static List<IHasBits> FilterList(this List<IHasBits> sourceList, Func<IHasBits, bool> filter)
        {
            List<IHasBits> rv = new List<IHasBits>();

            if (sourceList == null || sourceList.Count == 0)
                return rv;

            foreach (IHasBits each in sourceList)
            {
                if (filter(each))
                    rv.Add(each);
            }

            return rv;
        }

        public static Func<IHasBits, bool> BuildIsEquivalentToLogic(this IHasBits hasbits)
        {
            Func<IHasBits, bool> filter = (x) =>
            {
                return x.Bits.AreEquivalent_IntCompare(hasbits.Bits);
            };
            return filter;
        }
        /// <summary>
        /// returns true if the (item AND hasBits) == hasbits.  In other words, if the original mask has an ON bit
        /// @ position N, and the item shares this ON bit at the same position, the ANDed bit is also ON at position N.
        /// This is a masking function.  It returns those things that share the same ON bits as the mask.  This is not equivalence,
        /// which is when things share the same ON bits and OFF bits as the mask.
        /// </summary>
        /// <param name="hasbits"></param>
        /// <returns></returns>
        public static Func<IHasBits, bool> BuildANDLogic(this IHasBits hasbits)
        {
            Func<IHasBits, bool> filter = (x) =>
            {
                var newBits = x.AND(hasbits);

                return hasbits.Bits.AreEquivalent_IntCompare(newBits.Bits);
            };
            return filter;
        }
//        /// <summary>
//        /// performs an AND on each item in the list and if the result matches the mask, the original item is accumulated
//        /// </summary>
//        /// <param name="mask"></param>
//        /// <param name="items"></param>
//        /// <returns></returns>
//        public static List<IHasBits> ANDMatch(this IHasBits mask, List<IHasBits> items)
//        {
//            Condition.Requires(mask).IsNotNull();

//            List<IHasBits> rv = new List<IHasBits>();

//            if (items == null || items.Count == 0)
//                return rv;

//            int count = items.Count;
//            Parallel.For(0, count, () => new List<IHasBits>(),
//    (x, loop, subList) =>
//    {

//        return subList;
//    },
//    (x) => { rv.AddRange(x); }
//);

//            return rv;
//        }
    }

}
