using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using System.Linq.Expressions;

namespace Decoratid.Reflection
{
    /// <summary>
    /// Implementation of IObjectInspector
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReflectionObjectInspector : IObjectInspector
    {
        #region Ctor
        public ReflectionObjectInspector(Type type)
        {
            this.Type = type;
            this.Properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            this.Fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            this.GetterDelegates = new Dictionary<string, Func<object, object>>();
            this.SetterDelegates = new Dictionary<string, Action<object, object>>();

            this.Properties.WithEach(x =>
            {
                this.GetterDelegates[x.Name] = MakeGetterDelegate(type, x);
                this.SetterDelegates[x.Name] = MakeSetterDelegate(type, x);
            });
        }
        #endregion

        #region Properties
        public Type Type { get; private set; }
        public PropertyInfo[] Properties { get; private set; }
        public FieldInfo[] Fields { get; private set; }
        private Dictionary<string, Func<object, object>> GetterDelegates { get;  set; }
        private Dictionary<string, Action<object, object>> SetterDelegates { get;  set; }
        #endregion

        #region IObjectInspector
        public object GetProperty(object obj, string name)
        {
            return this.GetterDelegates[name].Invoke(obj);
        }

        public void SetProperty(object obj, string name, object value)
        {
            this.SetterDelegates[name].Invoke(obj, value);
        }
        #endregion

        #region Helpers
        static Action<object, object> MakeSetterDelegate(Type type, PropertyInfo property)
        {
            MethodInfo setMethod = property.GetSetMethod();
            if (setMethod != null && setMethod.GetParameters().Length == 1)
            {
                var target = Expression.Parameter(type);
                var value = Expression.Parameter(typeof(object));
                var body = Expression.Call(target, setMethod,
                    Expression.Convert(value, property.PropertyType));
                return Expression.Lambda<Action<object, object>>(body, target, value)
                    .Compile();
            }
            else
            {
                return null;
            }
        }
        static Func<object, object> MakeGetterDelegate(Type type, PropertyInfo property)
        {
            MethodInfo getMethod = property.GetGetMethod();
            if (getMethod != null )
            {
                var target = Expression.Parameter(type);
                var value = Expression.Parameter(typeof(object));
                var body = Expression.Call(target, getMethod);
                return Expression.Lambda<Func<object, object>>(body, target, value)
                    .Compile();
            }
            else
            {
                return null;
            }
        }
        #endregion
    }

    ////TODO: scrub url
    ////http://stackoverflow.com/questions/4085798/creating-an-performant-open-delegate-for-an-property-setter-or-getter?rq=1
    //abstract class Setter<T>
    //{
    //    public abstract void Set(T obj, object value);
    //}
    //class Setter<TTarget, TValue> : Setter<TTarget>
    //{
    //    private readonly Action<TTarget, TValue> del;
    //    public Setter(MethodInfo method)
    //    {
    //        del = (Action<TTarget, TValue>)
    //            Delegate.CreateDelegate(typeof(Action<TTarget, TValue>), method);
    //    }
    //    public override void Set(TTarget obj, object value)
    //    {
    //        del(obj, (TValue)value);
    //    }

    //}
    //abstract class Getter<T>
    //{
    //    public abstract object Get(T obj);
    //}
    //class Getter<TTarget, TValue> : Getter<TTarget>
    //{
    //    private readonly Func<TTarget, TValue> del;
    //    public Getter(MethodInfo method)
    //    {
    //        del = (Func<TTarget, TValue>)
    //            Delegate.CreateDelegate(typeof(Func<TTarget, TValue>), method);
    //    }
    //    public override object Get(TTarget obj)
    //    {
    //        return del(obj);
    //    }

    //}        
    //#region Helpers
    //    static Action<T, object> MakeSetterDelegate<T>(PropertyInfo property)
    //    {
    //        MethodInfo setMethod = property.GetSetMethod();
    //        if (setMethod != null && setMethod.GetParameters().Length == 1)
    //        {
    //            Setter<T> untyped = (Setter<T>)Activator.CreateInstance(
    //                typeof(Setter<,>).MakeGenericType(typeof(T),
    //                property.PropertyType), setMethod);
    //            return untyped.Set;
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }
    //    static Func<T, object> MakeGetterDelegate<T>(PropertyInfo property)
    //    {
    //        MethodInfo getMethod = property.GetGetMethod();
    //        if (getMethod != null)
    //        {
    //            Getter<T> untyped = (Getter<T>)Activator.CreateInstance(
    //                typeof(Getter<,>).MakeGenericType(typeof(T),
    //                property.PropertyType), getMethod);
    //            return untyped.Get;
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }
    //    #endregion
}
