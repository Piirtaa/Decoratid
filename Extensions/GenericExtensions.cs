using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Extensions
{
    public static class GenericExtensions
    {
        #region Inheritance
        /// <summary>
        /// given a generic type, determines whether the instance is of that generic type
        /// </summary>
        /// <param name="genericType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool IsInstanceOfGenericType(this Type genericTypeDefinition, object instance)
        {
            if (instance == null)
                return false;

            Type type = instance.GetType();
            return type.HasGenericDefinition(genericTypeDefinition);
        }

        /// <summary>
        /// Checks whether this type has the specified definition in its ancestry.
        /// </summary>   
        public static bool HasGenericDefinition(this Type type, Type genericTypeDefinition)
        {
            return GetTypeWithGenericDefinition(type, genericTypeDefinition) != null;
        }

        /// <summary>
        /// Walking the type's ancestry, returns the actual type implementing the generic type definition. Else, null.
        /// </summary>
        public static Type GetTypeWithGenericDefinition(this Type type, Type genericTypeDefinition)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (genericTypeDefinition == null)
                throw new ArgumentNullException("genericTypeDefinition");
            if (!genericTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException(
                    "The definition needs to be a GenericTypeDefinition", "definition");

            if (genericTypeDefinition.IsInterface)
                foreach (var interfaceType in type.GetInterfaces())
                    if (interfaceType.IsGenericType
                        && interfaceType.GetGenericTypeDefinition() == genericTypeDefinition)
                        return interfaceType;

            for (Type t = type; t != null; t = t.BaseType)
                if (t.IsGenericType && t.GetGenericTypeDefinition() == genericTypeDefinition)
                    return t;

            return null;
        }
        public static Type GetGenericParameterType(this Type type, Type genericTypeDefinition)
        {
            var genType = type.GetTypeWithGenericDefinition(genericTypeDefinition);
            if(genType != null)
                return genType.GetGenericArguments()[0];

            return null;
        }
        #endregion

        
    }
}
