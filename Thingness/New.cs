using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Storidiom.Extensions;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Storidiom.Thingness
{
    //from http://stackoverflow.com/questions/390578/creating-instance-of-type-without-default-constructor-in-c-sharp-using-reflectio
    /// <summary>
    /// helper that will instantiate objects for us. Doesn't use ctors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class New<T>
    {
        public static readonly Func<T> Instance = Creator();

        static Func<T> Creator()
        {
            Type t = typeof(T);
            if (t == typeof(string))
                return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();

            if (t.HasDefaultConstructor())
                return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();

            return () => (T)FormatterServices.GetUninitializedObject(t);
        }
    }
    /// <summary>
    /// Non generic version of New
    /// </summary>
    public static class New
    {
        public static object Create(Type type)
        {
            var builderType = typeof(New<>).MakeGenericType(type);
            var fi = builderType.GetField("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            //get the object
            var val = fi.GetValue(null);

            //convert to delegate and invoke
            Delegate del = (Delegate)val;
            var rv = del.DynamicInvoke();
            return rv;
        }
    }
}
