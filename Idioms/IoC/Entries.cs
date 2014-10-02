using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using System;

namespace Decoratid.Idioms.IoC
{
    /// <summary>
    /// a named instance/singleton IoC entry - keyed by name.  
    /// </summary>
    public class NamedEntry : IHasId<string>
    {
        #region Ctor
        public NamedEntry(string id, object instance)
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            this.Id = id;

            Condition.Requires(instance).IsNotNull();
            this.Instance = instance;
        }
        #endregion

        #region Properties
        object Instance { get; set; }
        public string Id { get; protected set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Methods
        public object GetInstance()
        {
            return this.Instance;
        }
        #endregion
    }

    /// <summary>
    /// an IoC entry keyed by type (eg. ILogger) 
    /// </summary>
    public class TypedEntry : IHasId<Type>
    {
        #region Ctor
        public TypedEntry(Type idType, Type instanceType)
        { 
            Condition.Requires(idType).IsNotNull();
            Condition.Requires(instanceType).IsNotNull();

            this.Id = idType;
            this.InstanceType = instanceType;
        }
        #endregion

        #region IIoCEntry
        public Type InstanceType {get;set;}
        public Type Id { get; protected set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Methods
        public object GetInstance()
        {
            return Activator.CreateInstance(this.InstanceType);
        }
        #endregion
    }

    /// <summary>
    /// an IoC entry that is keyed by type and uses a factory function to create the instance
    /// </summary>
    public class FactoriedEntry : IHasId<Type>
    {
        #region Ctor
        public FactoriedEntry(Type idType, Func<object> factory)
        {
            Condition.Requires(factory).IsNotNull();
            Condition.Requires(idType).IsNotNull();
            this.Id = idType;
            this.Factory = factory;
        }
        #endregion

        #region Properties
        public Func<object> Factory { get; set; }
        public Type Id { get; protected set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Methods
        public object GetInstance()
        {
            return this.Factory();
        }
        #endregion
    }

    /// <summary>
    /// an IoC entry that is keyed by type and uses a factory function (taking a context object - the caller) to create the instance
    /// </summary>
    public class ContextualFactoriedEntry : IHasId<Type>
    {
        #region Ctor
        public ContextualFactoriedEntry(Type idType, Func<object, object> factory)
        {
            Condition.Requires(factory).IsNotNull();
            Condition.Requires(idType).IsNotNull();
            this.Id = idType;
            this.Factory = factory;
        }
        #endregion

        #region Properties
        public Func<object, object> Factory { get; set; }
        public Type RegisteredType { get; set; }
        public Type Id { get; protected set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Methods
        public object GetInstance(object context)
        {
            return this.Factory(context);
        }
        #endregion
    }
}
