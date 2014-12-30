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
    [DebuggerStepThrough]
    public static class EnumerableExtensions
    {
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
            if(obj != null)
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
