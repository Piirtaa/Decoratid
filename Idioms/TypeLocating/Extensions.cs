using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TypeLocating
{
    public static class Extensions
    {
        /// <summary>
        /// looks for the type using the assembly qualified type name
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Type FindAssemblyQualifiedType(this ITypeLocator locator, string name)
        {
            if (locator == null)
                return null;

            if (string.IsNullOrEmpty(name))
                return null;

            var compoundType = locator.Locate((x) => { return x.AssemblyQualifiedName == name; });
            Type type = null;
            if (compoundType.Count == 0)
            {
                type = Type.GetType(name);
            }
            else
            {
                type = compoundType.FirstOrDefault();
            }

            return type;
        }
    }
}
