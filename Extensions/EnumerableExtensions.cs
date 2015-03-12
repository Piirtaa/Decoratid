using CuttingEdge.Conditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Extensions
{
    /// <summary>
    /// Extensions that operate on enumerables 
    /// </summary>
    /// 
    //[DebuggerStepThrough]
    public static class EnumerableExtensions
    {
        /// <summary>
        /// given some text, looks to find the matching suffix in the provided items list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T[] FindMatchingSuffix<T>(this T[] source, T[][] items)
        {
            foreach (T[] each in items)
                if (source.EndsWithSegment(each))
                    return each;

            return null;
        }
        /// <summary>
        /// given some text, looks to find the matching prefix in the provided items list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="currentPosition"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static T[] FindMatchingPrefix<T>(this T[] source, int currentPosition, T[][] items)
        {
            var substring = source.GetSegment(currentPosition);

            foreach (T[] each in items)
                if (substring.StartsWithSegment(each))
                    return each;

            return null;
        }

        /// <summary>
        /// Parsing helper.  Looks for the next "right" segment such that the count of "left" and "right" segments
        /// are equal.  This ensures we get a well-formed bracketing of left and right.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public static int GetPositionOfComplement<T>(this T[] source, T[] left, T[] right, int startPos = 0)
        {
            //  Condition.Requires(source.StartsWithSegment(left)).IsTrue();

            int pos = startPos;
            int unmatchedpairs = 0;
            for (int i = startPos; i < source.Length;)
            {
                var currentChar = source[i];
                pos = i;

                if (source.StartsWithSegment(left, i))
                {
                    unmatchedpairs++;
                    i += left.Length;
                }
                else if (source.StartsWithSegment(right, i))
                {
                    unmatchedpairs--;
                    i += right.Length;
                }
                else
                {
                    i++;
                }

                if (unmatchedpairs == 0)
                {
                    pos = i;
                    break;
                }
            }
            return pos;
        }

        /// <summary>
        /// does the source start with the prefix at the given position
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="prefix"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public static bool StartsWithSegment<T>(this T[] source, T[] prefix, int startPos = 0)
        {
            if (source == null)
                return false;

            if (prefix == null)
                return false;

            Condition.Requires(source).IsLongerOrEqual(prefix.Length);

            for (int i = 0; i < prefix.Length; i++)
                if (!source[startPos + i].Equals(prefix[i]))
                    return false;

            return true;
        }

        public static bool EndsWithSegment<T>(this T[] source, T[] suffix)
        {
            if (source == null)
                return false;

            if (suffix == null)
                return false;

            Condition.Requires(source).IsLongerOrEqual(suffix.Length);
            int length = source.Length;

            for (int i = 0; i < suffix.Length; i++)
                if (!source[length - i - 1].Equals(suffix[suffix.Length - i - 1]))
                    return false;

            return true;
        }
        public static T[] GetSegment<T>(this T[] source, int startPos, int length = -1)
        {
            int actualLength = length;
            if (length == -1)
                actualLength = source.Length - startPos;

            Condition.Requires(source).IsLongerOrEqual(startPos + actualLength);

            List<T> list = new List<T>();
            for (int i = startPos; i < (startPos + actualLength); i++)
            {
                list.Add(source[i]);
            }

            return list.ToArray();
        }
        /// <summary>
        /// finds first position of segment in source 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static int FindIndexOf<T>(this T[] source, T[] segment, int startPos = 0)
        {
            if (source == null)
                return -1;

            if (segment == null)
                return -1;

            int rv = -1;
            int segLength = segment.Length;

            for (int i = startPos; i < source.Length; i++)
            {
                T current = source[i];

                if (!segment[0].Equals(current))
                    continue;

                bool isGood = true;
                for (int j = 1; j < segLength; j++)
                {
                    if (!segment[j].Equals(source[i + j]))
                    {
                        isGood = false;
                        break;
                    }
                }
                if (isGood)
                {
                    rv = i;
                    break;
                }
            }

            return rv;
        }

        /// <summary>
        /// finds the nearest segment
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="segments"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public static int FindNearestIndexOf<T>(this T[] source, T[][] segments, out T[] segment, int startPos = 0)
        {
            int closestIdx = -1;
            T[] match = null;

            foreach (var each in segments)
            {
                //always search ahead of the current position
                var tempIdx = source.FindIndexOf(each, startPos + 1);

                //can't find the suffix, move along
                if (tempIdx == -1)
                    continue;

                //set the closest index if it's undefined
                if (closestIdx == -1)
                {
                    closestIdx = tempIdx;
                    match = each;
                    continue;
                }
                //update the closest index
                if (tempIdx < closestIdx)
                {
                    closestIdx = tempIdx;
                    match = each;
                    continue;
                }
            }

            segment = match;
            return closestIdx;
        }
        /// <summary>
        /// Returns the enumerable type (eg. IEnumerable of T would return T) fo the provided type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetEnumerableType(this Type type)
        {
            foreach (Type intType in type.GetInterfaces())
            {
                if (intType.IsGenericType
                    && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return intType.GetGenericArguments()[0];
                }
            }
            return null;
        }
        /// <summary>
        /// returns the indexed item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T GetIndex<T>(this IList<T> source, int index)
        {
            if (source == null || source.Count < index) { return default(T); }
            return source[index];
        }
        /// <summary>
        /// returns all items that return true from the filter. if the filter is null, returns the original list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<T> Filter<T>(this IList<T> source, Func<T, bool> filter)
        {
            if (source == null) { return null; }

            List<T> returnValue = new List<T>();

            source.WithEach(x =>
            {
                if (filter == null || filter(x))
                    returnValue.Add(x);
            });

            return returnValue;
        }
        ///// <summary>
        ///// converts a list of From types to a list of To types.  //From must derive from To
        ///// </summary>
        ///// <typeparam name="To"></typeparam>
        ///// <typeparam name="From"></typeparam>
        ///// <param name="source"></param>
        ///// <returns></returns>
        //public static List<To> ConvertListTo<To,From>(this List<From> source) 
        //    where From: class
        //    where To: class
        //{
        //    if (source == null) { return null; }

        //    List<To> returnValue = new List<To>();

        //    source.WithEach(x =>
        //    {
        //        if (x is To)
        //        {
        //            To t = x as To;
        //            returnValue.Add(t);
        //        }
        //    });

        //    return returnValue;
        //}

        /// <summary>
        /// iterates thru each item and performs the action.  doesn't kack on nulls
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void WithEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) { return; }
            if (action == null) { return; }

            foreach (T item in source)
            {
                action(item);
            }
        }
        /// <summary>
        /// iterates thru each item and performs the action until the break condition happens.  doesn't kack on nulls
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <param name="breakCondition"></param>
        public static void WithEach<T>(this IEnumerable<T> source, Action<T> action, Func<T, bool> breakCondition)
        {
            if (source == null) { return; }
            if (action == null) { return; }

            foreach (T item in source)
            {
                if (breakCondition != null)
                {
                    if (breakCondition(item))
                    {
                        break;
                    }
                }
                action(item);
            }
        }

        public static List<T> AddToList<T>(this T obj)
        {
            List<T> rv = new List<T>();
            if (obj != null)
                rv.Add(obj);
            return rv;
        }
        public static object GetEnumerableItemByIndex(this IEnumerable source, int index)
        {
            if (source == null) { return null; }
            int currIdx = 0;

            foreach (object item in source)
            {
                if (currIdx == index)
                    return item;
                currIdx++;
            }
            return null;
        }
    }
}
