using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Store;
using Sandbox.Store.Implementations;

namespace Sandbox.Reflection
{
    /// <summary>
    /// singleton container of cached type meta data
    /// </summary>
    public class TypeMetaCache
    {
        #region Declarations
        private readonly object _stateLock = new object(); //the explicit object we thread lock on 
        private static TypeMetaCache _instance = new TypeMetaCache(); //the singleton instance
        private FactoriedEvictingInMemoryStore _cache;
        #endregion

        #region Ctor
        static TypeMetaCache()
        {
        }
        private TypeMetaCache()
        {

            lock (this._stateLock)
            {
                this._cache = new FactoriedEvictingInMemoryStore(
                (type) =>
                {
                    TypeMeta item = type as TypeMeta;
                    return new TypeMeta(item.Id);
                }, null,
                new InMemoryStore());
            }
        }
        #endregion

        #region Properties
        public static TypeMetaCache Instance { get; private set; }

        #endregion

        #region Methods
        public TypeMeta GetTypeMeta(Type type)
        {
            return this._cache.Get<TypeMeta>(new StoredObjectId(typeof(TypeMeta), type));
        }
        #endregion
    }
}
