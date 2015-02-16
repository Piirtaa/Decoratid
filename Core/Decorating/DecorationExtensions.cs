﻿using CuttingEdge.Conditions;
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
        public static object WalkDecorationsUntilConditionMet(this object obj, Func<object, bool> filter)
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
        public static object As(this object obj, Type decType, bool exactTypeMatch = true)
        {
            var match = WalkDecorationsUntilConditionMet(obj, (dec) =>
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
        public static T As<T>(this object obj, bool exactTypeMatch = true)
        {
            var rv = obj.As(typeof(T), exactTypeMatch);
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

            var match = obj.WalkDecorationsUntilConditionMet((reg) =>
            {
                returnValue.Add(reg);
                return false;
            });

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

            var dec = obj.As(decType, exactTypeMatch);
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

        #region Facet Search
        //we want to find all decorations of a given type
        public static List<object> GetAllImplementingDecorations(this object obj, Type typeToImplement)
        {
            List<object> rv = new List<object>();

            var list = obj.GetAllDecorations();
            foreach (var each in list)
            {
                if (typeToImplement.IsInstanceOfType(each))
                    rv.Add(each);
            }

            return rv;
        }
        //we want to find all decorations of a given type
        public static List<T> GetAllImplementingDecorations<T>(this object obj)
        {
            Type type = typeof(T);
            var list = obj.GetAllImplementingDecorations(type);
            var castList = list.ConvertListTo<T,object>();
            return castList;
        }
        #endregion

        #region Decoration Signatures
        ///// <summary>
        ///// if the object is a decoration returns a | separated list of decoration id's, from 
        ///// outermost to innermost, excluding the core decorated thing.  
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static string GetExactDecorationSignature(this object obj)
        //{
        //    var list = obj.GetAllDecorations();

        //    List<string> ids = new List<string>();
        //    list.WithEach(x =>
        //    {
        //        var id = x as IDecoration;
        //        if(id != null)
        //            ids.Add(id.DecorationId.ToString());
        //    });
        //    if (ids.Count == 0)
        //        return string.Empty;

        //    var rv = string.Join("|", ids.ToArray());
        //    return rv;
        //}

        ///// <summary>
        ///// if the object is a decoration returns a | separated list of decoration id's, from 
        ///// outermost to innermost, excluding the core decorated thing, arranged alphabetically.
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static string GetAlphabeticDecorationSignature(this object obj)
        //{
        //    var list = obj.GetAllDecorations();

        //    List<string> ids = new List<string>();
        //    list.WithEach(x =>
        //    {
        //        var id = x as IDecoration;
        //        if (id != null)
        //            ids.Add(id.DecorationId.ToString());
        //    });
        //    if (ids.Count == 0)
        //        return string.Empty;

        //    ids.Sort();//sort by alpha

        //    var rv = string.Join("|", ids.ToArray());
        //    return rv;
        //}
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
