using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;


namespace Decoratid.Idioms.TypeLocating
{
    /// <summary>
    /// Container of types of a given type that are concrete and constructable, at a minimum, and also meet the provided filter.
    /// </summary>
    /// <remarks>
    /// Wraps a public writeable store of Types, in case one wants to decorate the store via a wrap/replace. 
    /// </remarks>
    public class TypeContainer<T>
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public TypeContainer(ITypeLocator locator, Func<Type, bool> filter = null)
        {
            Condition.Requires(locator).IsNotNull();
            this.Locator = locator;

            this.TypeStore = new StoreOfDecoration<AsId<Type>>(new NaturalInMemoryStore());
            this.RegisterTypes();
        }
        #endregion

        #region Properties
        public ITypeLocator Locator { get; private set; }
        public IStoreOf<AsId<Type>> TypeStore { get; private set; }
        #endregion

        #region Calculated Properties
        public List<Type> ContainedTypes
        {
            get
            {
                var list = this.TypeStore.GetAll<AsId<Type>>();
                List<Type> returnValue = new List<Type>();
                list.WithEach(x =>
                {
                    returnValue.Add(x.Id);
                });
                return returnValue;
            }
        }
        #endregion

        #region Type Registration Methods
        protected bool IsValidType(Type type)
        {
            return type.IsAbstract == false && type.IsInterface == false && typeof(T).IsAssignableFrom(type);
        }
        protected List<Type> RegisterTypes(Func<Type, bool> filter = null)
        {

            var list = this.Locator.Locate((type) =>
            {
                return this.IsValidType(type) && (filter == null ? true : filter(type));
            });
            list.WithEach(x =>
            {
                lock (this._stateLock)
                {
                    this.TypeStore.SaveItem(new AsId<Type>(x));
                }
            });
            return list;
        }
        #endregion

        #region Static Fluent
        public static TypeContainer<T> New(ITypeLocator locator, Func<Type, bool> filter = null)
        {
            return new TypeContainer<T>(locator, filter);
        }
        public static TypeContainer<T> NewDefault(Func<Type, bool> filter = null)
        {
            return new TypeContainer<T>(NaturalTypeLocator.New(), filter);
        }
        #endregion
    }
}
