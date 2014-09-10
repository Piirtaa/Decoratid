using Decoratid.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness.Idioms.Hydrateable
{
    /// <summary>
    /// Utility class that provides hydrateable helpers
    /// </summary>
    public static class HydrateableUtil
    {
        /// <summary>
        /// dehydrates hydrateable into the type prefix format
        /// </summary>
        /// <param name="hyd"></param>
        /// <returns></returns>
        public static string Dehydrate(IHydrateable hyd)
        {
            if (hyd == null)
                return null;
            
            string data = hyd.Dehydrate();
            return TypePrefixHydrateableUtil.Encode(hyd.GetType(), data);
        }

        /// <summary>
        /// Reconstitutes an object from the dehydrated, type-prefixed state, using the Hydrate mechanism
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static object Hydrate(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            Type type;
            var data =TypePrefixHydrateableUtil.Decode(text, out type);
            
            //check for null
            if (type == null)
                return null;

            var obj = ReflectionUtil.CreateUninitializedObject(type);
            IHydrateable hyd = obj as IHydrateable;
            hyd.Hydrate(data);

            return obj;
        }
    }
}
