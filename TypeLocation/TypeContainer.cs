using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Thingness.Idioms.Conditions.Common;
using Decoratid.Extensions;
using Decoratid.Thingness.Idioms.Store;
using Decoratid.Thingness.Idioms.Store.CoreStores;
using Decoratid.Thingness.Idioms.Store.Decorations.StoreOf;
using System.Diagnostics;


namespace Decoratid.TypeLocation
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
        /// <summary>
        /// plugins will be filtered by generic type
        /// </summary>
        public TypeContainer()
        {
            this.TypeStore = new StoreOfDecoration<AsId<Type>>(new InMemoryStore());
            this.RegisterTypes();
        }
        /// <summary>
        /// plugins will be filtered by generic type AND must pass the filter (ie. return true to be part of the container)
        /// </summary>
        /// <param name="filter"></param>
        public TypeContainer(Func<Type, bool> filter)
        {
            Condition.Requires(filter).IsNotNull();
            this.RegisterTypes(filter);
        }
        //public TypeContainer(TypeContainerConfig config)
        //    : this ((t) =>
        //    {
        //        return config.IsMatch(t);
        //    })
        //{

        //}
        #endregion

        #region Properties
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

        /// <summary>
        /// finds all Types of T and registers them in the type list
        /// </summary>
        /// <returns></returns>
        /// 
        [DebuggerStepThrough]
        protected List<Type> RegisterTypes()
        {
            var locator = TypeLocator.Instance;
            var list = locator.Locate((type) =>
            {
                return this.IsValidType(type);
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
        /// <summary>
        /// finds all Types of T and registers them in the type list.  excludes types that fail the filter
        /// </summary>
        protected List<Type> RegisterTypes(Func<Type, bool> filter)
        {
            var list = TypeLocator.Instance.Locate((type) =>
            {
                return this.IsValidType(type) && filter(type);
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
        public static TypeContainer<T> New()
        {
            return new TypeContainer<T>();
        }
        public static TypeContainer<T> New(Func<Type, bool> filter)
        {
            return new TypeContainer<T>(filter);
        }
        #endregion
    }
}
