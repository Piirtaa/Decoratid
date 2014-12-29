using CuttingEdge.Conditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Core.Decorating
{
    public static class DecorationExtensions
    {
        #region Basic object decoration extensions.  GetTypeBeingDecorated, IsDecoration, GetDecorated
        /// <summary>
        /// returns the type being decorated. Eg. for instance of IDecorationOf T, it returns T 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Type GetTypeBeingDecorated(this object obj)
        {
            if (obj == null)
                return null;

            //validate we're on a decoration
            var genTypeDef = typeof(IDecorationOf<>);
            if (!genTypeDef.IsInstanceOfGenericType(obj))
                return null;

            //get the generic type we're decorating
            var genType = obj.GetType().GetTypeInfo().GenericTypeArguments[0];

            return genType;
        }
        /// <summary>
        /// tells us if the object is a decoration (eg. IDecorationOf instance)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsADecoration(this object obj)
        {
            ////validate we're on a decoration
            //var genTypeDef = typeof(IDecorationOf<>);
            //return genTypeDef.IsInstanceOfGenericType(obj);

            if (obj == null)
                return false;

            if (obj is IDecoration)
                return true;

            return false;
        }
        /// <summary>
        /// if an object is a decoration, returns the first decoration
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetDecorated(this object obj)
        {
            if (obj == null)
                return null;

            ////validate we're on a decoration
            //var genTypeDef = typeof(IDecorationOf<>);
            //if (!genTypeDef.IsInstanceOfGenericType(obj))
            //    return null;

            ////use reflection to get the "Decorated" property
            //PropertyInfo pi = obj.GetType().GetProperty("Decorated", BindingFlags.Instance | BindingFlags.Public);
            //Condition.Requires(pi).IsNotNull();
            //var rv = pi.GetValue(obj);
            //return rv;


            if (!(obj is IDecoration))
                return null;

            return (obj as IDecoration).Decorated;
        }
        #endregion

        #region FreeWalk Iteration - walks decoration regardless of decorated type
        /// <summary>
        /// Does a walk, but doesn't restrict the walk to Decorations of T.  Will walk all Decorations regardless of type.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        /// <remarks>
        /// If a decoration chain has a change of layer type (ie. we start off decorating T1 and at some point a decoration
        /// converts the thing to a T2, which itself is then decorated) this function will walk it. 
        /// </remarks>
        public static object WalkDecorations(this object obj, Func<object, bool> filter)
        {
            object currentLayer = obj;

            //iterate down
            while (currentLayer != null)
            {
                //check filter.  break/return
                if (filter(currentLayer))
                {
                    return currentLayer;
                }

                currentLayer = GetDecorated(currentLayer);
            }

            return null;
        }
        /// <summary>
        /// walks all decorations regardless of type and looks for the specific decoration by a type match
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="decType"></param>
        /// <param name="exactTypeMatch"></param>
        /// <returns></returns>
        public static object FindDecoration(this object obj, Type decType, bool exactTypeMatch = true)
        {
            var match = WalkDecorations(obj, (dec) =>
            {
                //do type level filtering first

                //if we're exact matching, the decoration has to be the same type
                if (exactTypeMatch && decType.Equals(dec.GetType()) == false)
                    return false;

                //if we're not exact matching, the decoration has to be Of the same type
                if (exactTypeMatch == false && (!(decType.IsAssignableFrom(dec.GetType()))))
                    return false;

                return true;

            });

            return match;
        }
        /// <summary>
        /// walks all decorations regardless of type and looks for the specific decoration by a type match
        /// </summary>
        public static T FindDecoration<T>(this object obj, bool exactTypeMatch = true)
        {
            var rv = obj.FindDecoration(typeof(T), exactTypeMatch);
            if (rv == null)
                return default(T);

            return (T)rv;
        }
        /// <summary>
        /// Gets all decorations regardless of decoration type continuity
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<object> GetAllDecorations(this object obj)
        {
            List<object> returnValue = new List<object>();

            var match = obj.WalkDecorations((reg) =>
            {
                returnValue.Add(reg);
                return false;
            });

            return returnValue;
        }
        /// <summary>
        /// if the object is an instance of IDecorationOf T, returns all T in that "decoration of" cake
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> GetAllDecorationsOf<T>(this object obj)
        {
            List<T> returnValue = new List<T>();

            if (obj.IsADecorationOf<T>())
            {
                IDecorationOf<T> dec = obj as IDecorationOf<T>;
                returnValue = dec.GetAllDecorationsOf();
            }

            return returnValue;
        }
        /// <summary>
        /// if an object is a decoration, determines if a decoration exists in its cake
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool HasDecoration(this object obj, Type decType, bool exactTypeMatch = true)
        {
            if (obj == null)
                return false;

            var dec = obj.FindDecoration(decType, exactTypeMatch);
            return dec != null;
        }
        public static bool HasDecoration<T>(this object obj, bool exactTypeMatch = true)
        {
            return obj.HasDecoration(typeof(T), exactTypeMatch);
        }
        /// <summary>
        /// does a non-exact type match on all decorations
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="decTypes"></param>
        /// <returns></returns>
        public static bool HasDecorations(this object obj, params Type[] decTypes)
        {
            if (obj == null)
                return false;

            var decs = obj.GetAllDecorations();
            List<Type> allDecTypes = new List<Type>();
            foreach (var dec in decs)
            {
                allDecTypes.Add(dec.GetType());
            }

            bool rv = true;

            //iterate thru all the decorations to look for
            foreach (var decType in decTypes)
            {
                bool isFound = false;
                foreach (var actualDecType in allDecTypes)
                {
                    if (decType.IsAssignableFrom(actualDecType))
                    {
                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                {
                    rv = false;
                    break;
                }
            }

            return rv;
        }

        public static bool HasDecorations<T1, T2>(this object obj)
        {
            return obj.HasDecorations(typeof(T1), typeof(T2));
        }
        public static bool HasDecorations<T1, T2, T3>(this object obj)
        {
            return obj.HasDecorations(typeof(T1), typeof(T2), typeof(T3));
        }
        public static bool HasDecorations<T1, T2, T3, T4>(this object obj)
        {
            return obj.HasDecorations(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }
        public static bool HasDecorations<T1, T2, T3, T4, T5>(this object obj)
        {
            return obj.HasDecorations(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        }
        public static bool HasDecorations<T1, T2, T3, T4, T5, T6>(this object obj)
        {
            return obj.HasDecorations(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        }
        public static bool HasDecorations<T1, T2, T3, T4, T5, T6, T7>(this object obj)
        {
            return obj.HasDecorations(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
        }
        #endregion

        #region Walk Iteration - walks decorations of T
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
        /// returns the first decoration that matches the filter. stops iterating if ever finds a null decorated - 
        /// no chain to traverse!. will never return the core item (layer 0)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static T WalkDecorationsOf<T>(this IDecorationOf<T> obj, Func<T, bool> filter)
        {
            T currentLayer = obj.This;

            //iterate down
            while (currentLayer != null)
            {
                //check filter.  break/return
                if (filter(currentLayer))
                {
                    return currentLayer;
                }

                //recurse if it's decorated
                if (currentLayer is IDecorationOf<T>)
                {
                    IDecorationOf<T> layer = (IDecorationOf<T>)currentLayer;
                    currentLayer = layer.Decorated;
                }
                else
                {
                    //not decorated, and fails the filter?  stop here
                    return default(T);
                }
            }

            return default(T);
        }
        /// <summary>
        /// walks the decorator hierarchy to find the one of the provided type, and matching the filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FindDecorationOf<T>(this IDecorationOf<T> obj, Type decType, bool exactTypeMatch)
        {
            var match = obj.WalkDecorationsOf((dec) =>
            {
                //do type level filtering first

                //if we're exact matching, the decoration has to be the same type
                if (exactTypeMatch && decType.Equals(dec.GetType()) == false)
                    return false;

                //if we're not exact matching, the decoration has to be Of the same type
                if (exactTypeMatch == false && (!(decType.IsAssignableFrom(dec.GetType()))))
                    return false;

                return true;

            });

            if (match == null)
            {
                return default(T);
            }

            return match;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tdec"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="exactTypeMatch"></param>
        /// <returns></returns>
        public static Tdec FindDecorationOf<Tdec, T>(this IDecorationOf<T> obj, bool exactTypeMatch)
    where Tdec : T
        {
            var rv = obj.FindDecorationOf(typeof(Tdec), exactTypeMatch);
            if (rv == null)
                return default(Tdec);

            return (Tdec)rv;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> GetAllDecorationsOf<T>(this IDecorationOf<T> obj)
        {
            List<T> returnValue = new List<T>();

            var match = obj.WalkDecorationsOf<T>((reg) =>
            {
                returnValue.Add(reg);
                return false;
            });

            return returnValue;
        }

        #endregion


        //public static object RemoveDecoration(Type decType, object obj)
        //{
        //    var decs = GetDecorationList(obj);

        //    //validate we're on a decoration
        //    var genTypeDef = typeof(DecorationOfBase<>);
        //    if (!genTypeDef.IsInstanceOfGenericType(obj))
        //        return null;

        //    //get the generic type we're decorating
        //    var genType = obj.GetType().GetTypeInfo().GenericTypeArguments[0];

        //    //get all the decorations via a reflection call 
        //    var hardDecType = genTypeDef.MakeGenericType(genType);
        //    MethodInfo mi = hardDecType.GetMethod("Undecorate", BindingFlags.Instance | BindingFlags.Public);
        //    Condition.Requires(mi).IsNotNull();
        //    var rv = mi.Invoke(obj, new object[] { decType, true });
        //    return rv;
        //}
    }
}
