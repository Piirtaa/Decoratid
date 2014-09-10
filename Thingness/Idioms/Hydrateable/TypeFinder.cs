using Decoratid.TypeLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness.Idioms.Hydrateable
{
    public static class TypeFinder
    {
        public static Type FindAssemblyQualifiedType(string name)
        {
            var compoundType = TypeLocator.Instance.Locate((x) => { return x.AssemblyQualifiedName == name; });
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
