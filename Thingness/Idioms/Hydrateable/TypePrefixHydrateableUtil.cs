using CuttingEdge.Conditions;
using Decoratid.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness.Idioms.Hydrateable
{
    /// <summary>
    /// Utility class that provides type-prefix hydrateable helpers
    /// </summary>
    public static class TypePrefixHydrateableUtil
    {
        /// <summary>
        /// If the text parses to a known type, etc, returns true.  otherwise false.  
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsCompliant(string text)
        {
            bool rv = false;
            var list = TextDecorator.LengthDecodeList(text);
            if (list == null)
                return rv;
            if (list.Count != 2)
                return rv;

            //pull the type out
            string typeName = list[0];
            var type = TypeFinder.FindAssemblyQualifiedType(typeName);
            if (type == null)
                return rv;

            if (!typeof(IHydrateable).IsAssignableFrom(type))
                return rv;

            rv = true;
            return rv;
        }
        /// <summary>
        /// encodes data with TypePrefixEncoding
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encode(Type type, string data)
        {
            Condition.Requires(type).IsNotNull();
            Condition.Requires(data).IsNotNullOrEmpty();
            return TextDecorator.LengthEncodeList(type.AssemblyQualifiedName, data);
        }
        /// <summary>
        /// decodes typeprefixencoding into its constituent parts:
        /// type and the encoded data
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type">returns the resolved Type</param>
        /// <returns>the data portion</returns>
        public static string Decode(string text, out Type type)
        {
            if (!IsCompliant(text))
                throw new InvalidOperationException("non compliant encoding");

            var list = TextDecorator.LengthDecodeList(text);
            string typeName = list[0];
            var outType = TypeFinder.FindAssemblyQualifiedType(typeName);
            type = outType;

            return list[1];
        }
        

    }
}
