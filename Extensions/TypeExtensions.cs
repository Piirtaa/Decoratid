using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Extensions
{
    public static class TypeExtensions
    {
        public static Type FindTypeByName(this List<Type> types, string typeName)
        {
            if (types != null && types.Count > 0)
            {
                foreach (var each in types)
                {
                    if (each.Name == typeName)
                    {

                    }
                }
            }
            return null;

            types.WithEach(x =>
            {

            }, (x) =>
            {

            });
        }
    }
}
