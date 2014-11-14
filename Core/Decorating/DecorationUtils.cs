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
    public static class DecorationUtils
    {
        /// <summary>
        /// gets the core type being decorated
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Type GetDecorationLayerType(object obj)
        {
            if (obj == null)
                return null;

            //validate we're on a decoration
            var genTypeDef = typeof(DecorationOfBase<>);
            if (!genTypeDef.IsInstanceOfGenericType(obj))
                return null;

            //get the generic type we're decorating
            var genType = obj.GetType().GetTypeInfo().GenericTypeArguments[0];

            return genType;
        }
        /// <summary>
        /// tells us if the object is a decoration
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDecoration(object obj)
        {
            //validate we're on a decoration
            var genTypeDef = typeof(IDecorationOf<>);
            return genTypeDef.IsInstanceOfGenericType(obj);
        }
        /// <summary>
        /// if an object is a decoration, returns the first decoration
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetDecorated(object obj)
        {
            if (obj == null)
                return null;

            //validate we're on a decoration
            var genTypeDef = typeof(IDecorationOf<>);
            if (!genTypeDef.IsInstanceOfGenericType(obj))
                return null;

            //use reflection to get the "Decorated" property
            PropertyInfo pi = obj.GetType().GetProperty("Decorated", BindingFlags.Instance | BindingFlags.Public);
            Condition.Requires(pi).IsNotNull();
            var rv = pi.GetValue(obj);
            return rv;
        }

        /// <summary>
        /// if an object is a decoration, returns the list of decorations outermost to core
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable GetDecorationList(object obj)
        {
            if (obj == null)
                return null;

            //validate we're on a decoration
            var genTypeDef = typeof(DecorationOfBase<>);
            if (!genTypeDef.IsInstanceOfGenericType(obj))
                return null;

            //get the generic type we're decorating
            var decOfType = obj.GetType().GetTypeWithGenericDefinition(genTypeDef);
            var objTypeInfo = decOfType.GetTypeInfo();
            var genType = objTypeInfo.GenericTypeArguments[0];

            //get all the decorations via a reflection call 
            var decType = genTypeDef.MakeGenericType(genType);
            PropertyInfo pi = decType.GetProperty("OutermostToCore", BindingFlags.Instance | BindingFlags.Public);
            Condition.Requires(pi).IsNotNull();
            var decList = pi.GetValue(obj);
            IEnumerable list = decList as IEnumerable;
            return list;
        }

        public static object GetDecoration(Type decType, object obj)
        {
            var decs = GetDecorationList(obj);

            foreach (var each in decs)
            {
                if (each.GetType().Equals(decType))
                    return each;
            }

            return null;
        }

        public static T GetDecoration<T>(object obj)
        {
            var rv = GetDecoration(typeof(T), obj);
            if (rv == null)
                return default(T);

            return (T)rv;
        }
        /// <summary>
        /// if an object is a decoration, determines if a decoration exists in its cake
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool HasDecoration(Type decType, object obj)
        {
            var decs = GetDecorationList(obj);
            if (decs == null)
                return false;

            foreach (var each in decs)
            {
                if (each.GetType().Equals(decType))
                    return true;
            }

            return false;
        }
        public static bool HasDecoration<T>(object obj)
        {
            return HasDecoration(typeof(T), obj);
        }

        public static object RemoveDecoration(Type decType, object obj)
        {
            var decs = GetDecorationList(obj);

            //validate we're on a decoration
            var genTypeDef = typeof(DecorationOfBase<>);
            if (!genTypeDef.IsInstanceOfGenericType(obj))
                return null;

            //get the generic type we're decorating
            var genType = obj.GetType().GetTypeInfo().GenericTypeArguments[0];

            //get all the decorations via a reflection call 
            var hardDecType = genTypeDef.MakeGenericType(genType);
            MethodInfo mi = hardDecType.GetMethod("Undecorate", BindingFlags.Instance | BindingFlags.Public);
            Condition.Requires(mi).IsNotNull();
            var rv = mi.Invoke(obj, new object[] { decType, true });
            return rv;
        }
    }
}
