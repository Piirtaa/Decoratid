using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Utils
{

    public static class ReflectionUtil
    {
        #region Elegant recursive get fields -> need to do this for serialization cos of default c# behav

        public static FieldInfo[] GetFieldInfosIncludingBaseClasses(Type type, BindingFlags bindingFlags)
        {
            FieldInfo[] fieldInfos = type.GetFields(bindingFlags);

            // If this class doesn't have a base, don't waste any time
            if (type.BaseType == typeof(object))
            {
                return fieldInfos;
            }
            else
            {   // Otherwise, collect all types up to the furthest base class
                var currentType = type;
                var fieldComparer = new FieldInfoComparer();
                var fieldInfoList = new HashSet<FieldInfo>(fieldComparer);
                if (fieldInfos != null && fieldInfos.Length > 0)
                    fieldInfoList.UnionWith(fieldInfos);

                while (currentType != null && currentType != typeof(object))
                {
                    fieldInfos = currentType.GetFields(bindingFlags);
                    if (fieldInfos != null && fieldInfos.Length > 0)
                        fieldInfoList.UnionWith(fieldInfos);
                    currentType = currentType.BaseType;
                }
                return fieldInfoList.ToArray();
            }
        }


        public class FieldInfoComparer : IEqualityComparer<FieldInfo>
        {
            public bool Equals(FieldInfo x, FieldInfo y)
            {
                return x.DeclaringType == y.DeclaringType && x.Name == y.Name;
            }

            public int GetHashCode(FieldInfo obj)
            {
                return obj.Name.GetHashCode() ^ obj.DeclaringType.GetHashCode();
            }
        }
        #endregion

        public static object CreateUninitializedObject(Type type)
        {
            return FormatterServices.GetUninitializedObject(type);
        }
        /// <summary>
        /// Creates an object instance from a type name.  Uses Type.GetType to locate the type.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static object CreateObject(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) { return null; }

            try
            {
                //load the type up
                var type = Type.GetType(typeName);
                if (type == null) { return null; }

                return Activator.CreateInstance(type);
            }
            catch { }
            return null;
        }
        /// <summary>
        /// if a field is a backing field of an autoimplemented property, we return the name of the property.
        /// depends on the compiler, so it is fragile.
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        public static string GetAutoimplementedPropertyName(FieldInfo fi)
        {
            if (!fi.IsDefined(typeof(CompilerGeneratedAttribute), false))
                return null;

            string prefix = "<";
            string suffix = ">k__BackingField";
            if (fi.Name.StartsWith(prefix) &&
                fi.Name.EndsWith(suffix))
            {
                return fi.Name.Substring(1, fi.Name.IndexOf(suffix));
            }

            return null;
        }
    }
}
