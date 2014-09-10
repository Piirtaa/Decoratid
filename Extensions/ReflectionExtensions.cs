using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Decoratid.Extensions
{
    public static class ReflectionExtensions
    {
        #region PropertyInfo
        public static bool HasPublicGetter(this PropertyInfo pi)
        {
            return pi.CanRead && pi.GetGetMethod() != null;
        }
        public static bool HasPublicSetter(this PropertyInfo pi)
        {
            return pi.CanWrite && pi.GetSetMethod() != null;
        }

        #endregion



        public static bool HasDefaultConstructor(this Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
