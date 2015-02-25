using CuttingEdge.Conditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core.Identifying;

namespace Decoratid.Core.Decorating
{
    public static class DecorationOfExtensions
    {
        public static bool IsADecorationOf(this object obj)
        {
            if (obj == null)
                return false;

            //validate we're on a decoration
            var genTypeDef = typeof(IDecorationOf<>);
            return genTypeDef.IsInstanceOfGenericType(obj);
        }
        /// <summary>
        /// returns whether the object is a decoration of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsADecorationOf<T>(this object obj)
        {
            if (obj == null)
                return false;

            if (obj is IDecorationOf<T>)
                return true;

            return false;
        }
        /// <summary>
        /// returns the type being decorated. Eg. for instance of IDecorationOf T, it returns T 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Type GetTypeBeingDecorated(this Type objType)
        {
            if (objType == null)
                return null;

            //validate we're on a decoration
            if (!(obj is IDecoration))
                return null;

            //var genTypeDef = typeof(IDecorationOf<>);
            //if (!genTypeDef.IsInstanceOfGenericType(obj))
            //    return null;

            //get the generic type we're decorating
            var genType = obj.GetType().GetInterface("IDecorationOf`1").GetGenericArguments()[0];
            //var genType = obj.GetType().GetTypeInfo().GenericTypeArguments[0];

            return genType;
        }

        #region Walk Iteration - walks decorations of T

    //    /// <summary>
    //    /// returns the first decoration that matches the filter. stops iterating if ever finds a null decorated - 
    //    /// no chain to traverse!. will never return the core item (layer 0)
    //    /// </summary>
    //    /// <param name="filter"></param>
    //    /// <returns></returns>
    //    public static T WalkDecorationsOf<T>(this IDecorationOf<T> obj, Func<T, bool> filter)
    //    {
    //        T currentLayer = obj.This;

    //        //iterate down
    //        while (currentLayer != null)
    //        {
    //            //check filter.  break/return
    //            if (filter(currentLayer))
    //            {
    //                return currentLayer;
    //            }

    //            //recurse if it's decorated
    //            if (currentLayer is IDecorationOf<T>)
    //            {
    //                IDecorationOf<T> layer = (IDecorationOf<T>)currentLayer;
    //                currentLayer = layer.Decorated;
    //            }
    //            else
    //            {
    //                //not decorated, and fails the filter?  stop here
    //                return default(T);
    //            }
    //        }

    //        return default(T);
    //    }
    //    /// <summary>
    //    /// walks the decorator hierarchy to find the one of the provided type, and matching the filter
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <returns></returns>
    //    public static T FindDecorationOf<T>(this IDecorationOf<T> obj, Type decType, bool exactTypeMatch)
    //    {
    //        var match = obj.WalkDecorationsOf((dec) =>
    //        {
    //            //do type level filtering first

    //            //if we're exact matching, the decoration has to be the same type
    //            if (exactTypeMatch && decType.Equals(dec.GetType()) == false)
    //                return false;

    //            //if we're not exact matching, the decoration has to be Of the same type
    //            if (exactTypeMatch == false && (!(decType.IsAssignableFrom(dec.GetType()))))
    //                return false;

    //            return true;

    //        });

    //        if (match == null)
    //        {
    //            return default(T);
    //        }

    //        return match;
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <typeparam name="Tdec"></typeparam>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="obj"></param>
    //    /// <param name="exactTypeMatch"></param>
    //    /// <returns></returns>
    //    public static Tdec FindDecorationOf<Tdec, T>(this IDecorationOf<T> obj, bool exactTypeMatch)
    //where Tdec : T
    //    {
    //        var rv = obj.FindDecorationOf(typeof(Tdec), exactTypeMatch);
    //        if (rv == null)
    //            return default(Tdec);

    //        return (Tdec)rv;
    //    }
    //    /// <summary>
    //    /// gets the layer cake of T, outermost to core
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="obj"></param>
    //    /// <returns></returns>
    //    public static List<T> GetAllDecorationsOf<T>(this IDecorationOf<T> obj)
    //    {
    //        List<T> returnValue = new List<T>();

    //        var match = obj.WalkDecorationsOf<T>((reg) =>
    //        {
    //            returnValue.Add(reg);
    //            return false;
    //        });

    //        return returnValue;
    //    }
    //    /// <summary>
    //    /// if the object is an instance of IDecorationOf T, returns all T in that "decoration of" cake
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="obj"></param>
    //    /// <returns></returns>
    //    public static List<T> GetAllDecorationsOf<T>(this object obj)
    //    {
    //        List<T> returnValue = new List<T>();

    //        if (obj.IsADecorationOf<T>())
    //        {
    //            IDecorationOf<T> dec = obj as IDecorationOf<T>;
    //            returnValue = dec.GetAllDecorationsOf();
    //        }

    //        return returnValue;
    //    }
        #endregion

    }
}
