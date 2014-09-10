using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Store;

namespace Decoratid.Reflection
{
    /// <summary>
    /// contains type meta data
    /// </summary>
    public class TypeMeta : IHasId<Type>
    {
        #region Ctor
        public TypeMeta(Type type)
        {
            this.Type = type;
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            props.WithEach(pi =>
            {
                this.Properties.Add(new PropertyMeta(pi));
            });
        }
        #endregion

        #region Properties
        public Type Id
        {
            get { return this.Type; }
        }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        public Type Type { get; private set; }
        public List<PropertyMeta> Properties { get; private set; }
        #endregion

        #region Methods
        public PropertyMeta GetProperty(string name)
        {
            return this.Properties.SingleOrDefault(x => x.Name == name);
        }
        #endregion


    }

    public class PropertyMeta
    {
        #region Ctor
        public PropertyMeta(PropertyInfo pi)
        {
            Condition.Requires(pi).IsNotNull();
            this.PropertyInfo = pi;
            this.Name = pi.Name;
            this.Type = pi.PropertyType;
            this.CanGet = pi.HasPublicGetter();
            this.CanSet = pi.HasPublicSetter();
        }
        #endregion

        #region Properties
        public string Name { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public Type Type { get; private set; }
        public bool CanGet { get; private set; }
        public bool CanSet { get; private set; }
        #endregion
    }
}
