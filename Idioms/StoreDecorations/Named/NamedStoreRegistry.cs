using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Store.Decorations.Named
{
    /// <summary>
    /// Singleton store of registries
    /// </summary>
    public class NamedStoreRegistry
    {
                #region Inner Classes
        /// <summary>
        /// the object we're brokering needs an Id - so we wrap it here
        /// </summary>
        public class ObjectInspectorItem : IHasId<Type>
        {
            public ObjectInspectorItem(Type type)
            {
                this.Id = type;
                this.Inspector = new ReflectionObjectInspector(type);
            }
            public IObjectInspector Inspector {get;set;}
            public Type Id
            {
                get;
                private set;
            }

            object IHasId.Id
            {
                get { return this.Id; }
            }
        }
        #endregion

        #region Declarations
        private readonly object _stateLock = new object(); //the explicit object we thread lock on 
        private static ObjectInspectorCache _instance = new ObjectInspectorCache(); //the singleton instance
        private FactoriedEvictingInMemoryStore _cache;
        #endregion

        #region Ctor
        static ObjectInspectorCache()
        {
        }
        private ObjectInspectorCache()
        {
            lock (this._stateLock)
            {
                this._cache = new FactoriedEvictingInMemoryStore(
                (type) =>
                {
                    //create the data wrapper class (defined above), that holds our inspector
                    ObjectInspectorItem item = type as ObjectInspectorItem;
                     return new ObjectInspectorItem(item.Id);
                },
                new InMemoryStore());
            }
        }
        #endregion

        #region Properties
        public static ObjectInspectorCache Instance { get; private set; }
        #endregion

        #region Methods
        public IObjectInspector GetInspector(Type type)
        {
            var item = this._cache.Get<ObjectInspectorItem>(new ObjectInspectorItem(type));
        
            return item.Inspector;
        }
        #endregion
    }
}
