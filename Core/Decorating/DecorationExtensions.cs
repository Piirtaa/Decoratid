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
    public static class IDecorationExtensions
    {
        /// <summary>
        /// tells us if the object is a IDecoration
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsADecoration(this object obj)
        {
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

            if (!(obj is IDecoration))
                return null;

            return (obj as IDecoration).Decorated;
        }
        /// <summary>
        /// walking Decorated chain, gets the Core 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetCoreDecorated(this object obj)
        {
            object last = obj;
            object currentLayer = obj;

            //iterate down
            while (currentLayer != null)
            {
                currentLayer = GetDecorated(currentLayer);
                if (currentLayer != null)
                    last = currentLayer;
            }

            return (last as IDecoration).Decorated;
        }
        /// <summary>
        /// walks Decorated chain to the core, or until the stop condition is met
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stopWalkCondition"></param>
        /// <returns></returns>
        public static object WalkDecorationsToCore(this object obj, Func<object, bool> stopWalkCondition)
        {
            object currentLayer = obj;

            //iterate down
            while (currentLayer != null)
            {
                //check filter.  break/return
                if (stopWalkCondition(currentLayer))
                {
                    return currentLayer;
                }

                currentLayer = GetDecorated(currentLayer);
            }

            return null;
        }

        /// <summary>
        /// gets all the decorations, including the core, below this object including itself
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<object> GetSelfAndAllDecorationsBelow(this object obj)
        {
            List<object> returnValue = new List<object>();

            var match = obj.WalkDecorationsToCore((reg) =>
            {
                returnValue.Add(reg);
                return false;
            });

            return returnValue;
        }
    }

    /// <summary>
    /// basically the mirror of IDecorationExtensions but going Upwards on the Decorator chain, instead of 
    /// Downwards on the Decorated chain
    /// </summary>
    public static class IDecoratorAwareDecorationExtensions
    {
        public static bool IsADecoratorAwareDecoration(this object obj)
        {
            if (obj == null)
                return false;

            if (obj is IDecoratorAwareDecoration)
                return true;

            return false;
        }
        /// <summary>
        /// if this is stackaware it gets its decorator
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetDecorator(this object obj)
        {
            if (obj == null)
                return null;

            if (!(obj is IDecoratorAwareDecoration))
                return null;

            return (obj as IDecoratorAwareDecoration).Decorator;
        }
        /// <summary>
        /// Walks up the Decorator chain to the topmost
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetOuterDecorator(this object obj)
        {
            object last = obj;
            object currentLayer = obj;

            //iterate down
            while (currentLayer != null)
            {
                currentLayer = GetDecorator(currentLayer);
                if (currentLayer != null)
                    last = currentLayer;
            }

            return last;
        }
        /// <summary>
        /// walks towards the outwards (eg. towards the decorator)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stopWalkCondition"></param>
        /// <returns></returns>
        public static object WalkDecoratorsToOuter(this object obj, Func<object, bool> stopWalkCondition)
        {
            object currentLayer = obj;

            //iterate down
            while (currentLayer != null)
            {
                //check filter.  break/return
                if (stopWalkCondition(currentLayer))
                {
                    return currentLayer;
                }

                currentLayer = GetDecorator(currentLayer);
            }

            return null;
        }

        public static List<object> GetSelfAndAllDecoratorsAbove(this object obj)
        {
            List<object> returnValue = new List<object>();

            var match = obj.WalkDecoratorsToOuter((reg) =>
            {
                returnValue.Add(reg);
                return false;
            });

            return returnValue;
        }
    }

    /// <summary>
    /// extension methods that handle the "As" type conversions on a decoration cake
    /// </summary>
    public static class AsExtensions
    {
        /// <summary>
        /// looks for the "As face" by walking down the decorations 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="decorationType"></param>
        /// <param name="exactTypeMatch"></param>
        /// <returns></returns>
        public static object AsBelow(this object obj, Type decorationType, bool exactTypeMatch = true)
        {
            if (obj == null)
                return null;

            var match = IDecorationExtensions.WalkDecorationsToCore(obj, (dec) =>
            {
                //do type level filtering first

                //if we're exact matching, the decoration has to be the same type
                if (exactTypeMatch && decorationType.Equals(dec.GetType()) == false)
                    return false;

                //if we're not exact matching, the decoration has to be Of the same type
                if (exactTypeMatch == false && (!(decorationType.IsAssignableFrom(dec.GetType()))))
                    return false;

                return true;

            });

            return match;
        }
        public static T AsBelow<T>(this object obj, bool exactTypeMatch = true)
        {
            var rv = obj.AsBelow(typeof(T), exactTypeMatch);
            if (rv == null)
                return default(T);

            return (T)rv;
        }
        /// <summary>
        /// looks for the "As face" by walking up the decorators
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="decorationType"></param>
        /// <param name="exactTypeMatch"></param>
        /// <returns></returns>
        public static object AsAbove(this object obj, Type decorationType, bool exactTypeMatch = true)
        {
            if (obj == null)
                return null;

            var match = IDecoratorAwareDecorationExtensions.WalkDecoratorsToOuter(obj, (dec) =>
            {
                //do type level filtering first

                //if we're exact matching, the decoration has to be the same type
                if (exactTypeMatch && decorationType.Equals(dec.GetType()) == false)
                    return false;

                //if we're not exact matching, the decoration has to be Of the same type
                if (exactTypeMatch == false && (!(decorationType.IsAssignableFrom(dec.GetType()))))
                    return false;

                return true;

            });

            return match;
        }
        public static T AsAbove<T>(this object obj, bool exactTypeMatch = true)
        {
            var rv = obj.AsAbove(typeof(T), exactTypeMatch);
            if (rv == null)
                return default(T);

            return (T)rv;
        }

        /// <summary>
        /// looks for the "As face" by walking ALL the decorations.  If DecoratorAware, walks down from Outer.  If not
        /// DecoratorAware, walks down from Self 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="decorationType"></param>
        /// <param name="exactTypeMatch"></param>
        /// <returns></returns>
        public static object As(this object obj, Type decorationType, bool exactTypeMatch)
        {
            if (obj == null)
                return null;

            //if decorator aware get the outer
            object topMost = obj;

            if (IDecoratorAwareDecorationExtensions.IsADecoratorAwareDecoration(obj))
            {
                topMost = IDecoratorAwareDecorationExtensions.GetOuterDecorator(obj);
            }

            return AsBelow(topMost, decorationType, exactTypeMatch);
        }
        public static T As<T>(this object obj, bool exactTypeMatch = true)
        {
            var rv = obj.As(typeof(T), exactTypeMatch);
            if (rv == null)
                return default(T);

            return (T)rv;
        }

        /// <summary>
        /// If DecoratorAware, walks down from Outer.  If not DecoratorAware, walks down from Self 
        /// </summary>
        public static List<object> GetAllDecorations(this object obj)
        {
            List<object> rv = new List<object>();

            if (obj == null)
                return rv;

            //if decorator aware get the outer
            object topMost = obj;

            if (IDecoratorAwareDecorationExtensions.IsADecoratorAwareDecoration(obj))
            {
                topMost = IDecoratorAwareDecorationExtensions.GetOuterDecorator(obj);
            }

            rv = IDecorationExtensions.GetSelfAndAllDecorationsBelow(topMost);
            return rv;
        }

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
            var castList = list.ConvertListTo<T, object>();
            return castList;
        }
    }

    public static class HasExtensions
    {
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
    }

    public static class DecorationExtensions
    {
        public static bool ToggleDecoration(this object obj, bool isEnabled, Type decType, bool exactTypeMatch = true)
        {
            if (obj == null)
                return false;

            var dec = obj.As(decType, exactTypeMatch);

            if (dec == null)
                return false;

            if (dec is ITogglingDecoration)
            {
                (dec as ITogglingDecoration).IsDecorationEnabled = isEnabled;
                return true;
            }
            return false;
        }
        public static bool ToggleDecoration<T>(this object obj, bool isEnabled, bool exactTypeMatch = true)
        {
            return obj.ToggleDecoration(isEnabled, typeof(T), exactTypeMatch);
        }

        /// <summary>
        /// returns a textual decoration summary of "id, layers"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetDecorationSummary(this object obj)
        {
            if (obj == null)
                return null;

            var cake = obj.GetAllDecorations();
            var id = obj.As<IHasId>(false).With(x => x.Id.ToString());

            var rv = string.Format("{0}, {1}", id == null ? "<NULL>" : id, string.Join(", ", (from o in cake select o.GetType().Name).ToList()));
            return rv;
        }

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

    //TODO: enable/disable layer switch

}
