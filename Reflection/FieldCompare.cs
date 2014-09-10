using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Reflection
{
    //http://stackoverflow.com/a/986617

    public static class FieldCompare
    {
        public static bool IsEqual(object x, object y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            if (!x.GetType().Equals(y.GetType()))
                return false;

            //now perform the generic equals call
            var mi = typeof(FieldCompare).GetMethod("Equal", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            var gmi = mi.MakeGenericMethod(x.GetType());
            var res = gmi.Invoke(null, new object[] { x, y });

            return (bool)res;
        }

        private static bool Equal<T>(T x, T y)
        {
            var rv = Cache<T>.Compare(x, y);
            if (!rv)
            {
                var s = "Asda";
            }
            return rv;
        }
        static class Cache<T>
        {
            internal static readonly Func<T, T, bool> Compare;
            static Cache()
            {
                //unlike MemberCompare we compare all fields only
                var fields = ReflectionUtil.GetFieldInfosIncludingBaseClasses(typeof(T), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                var x = Expression.Parameter(typeof(T), "x");
                var y = Expression.Parameter(typeof(T), "y");

                Expression body = null;
                foreach (var field in fields)
                {
                    Expression memberEqual = Expression.Equal(
                                Expression.Field(x, field),
                                Expression.Field(y, field));
                    if (body == null)
                    {
                        body = memberEqual;
                    }
                    else
                    {
                        body = Expression.AndAlso(body, memberEqual);
                    }
                }
                if (body == null)
                {
                    Compare = delegate { return true; };
                }
                else
                {
                    Compare = Expression.Lambda<Func<T, T, bool>>(body, x, y)
                                  .Compile();
                }
            }
        }
    }
}
