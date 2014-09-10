using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Configuration;
using Decoratid.TypeLocation;
using Decoratid.Extensions;
using Decoratid.Thingness.Idioms.Store.Decorations.StoreOf;
using Decoratid.Thingness.Idioms.Store.CoreStores;

namespace Decoratid.Thingness.Idioms.Store.Broker
{
    /// <summary>
    /// Singleton container of IStoreConnectionBuilders.  Call this to create stores from connection strings.
    /// </summary>
    public class StoreConnectionManager
    {
        #region Declarations
        private readonly object _stateLock = new object(); //the explicit object we thread lock on
        #endregion

        #region Ctor
        static StoreConnectionManager()
        {
        }
        private StoreConnectionManager()
        {
            lock (this._stateLock)
            {
                this.TypeContainer = new TypeContainer<IStoreConnectionBuilder>();
                this.Store = new StoreOfDecoration<IStoreConnectionBuilder>(new InMemoryStore());

                this.TypeContainer.ContainedTypes.WithEach(x =>
                {
                    IStoreConnectionBuilder obj = Activator.CreateInstance(x) as IStoreConnectionBuilder;
                    if (obj != null)
                        this.Store.SaveItem(obj);
                });
            }
        }
        #endregion

        #region Properties
        private TypeContainer<IStoreConnectionBuilder> TypeContainer { get; set; }
        private IStoreOf<IStoreConnectionBuilder> Store { get; set; }
        public static StoreConnectionManager Instance { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// returns the first store that has the same storeName
        /// </summary>
        /// <param name="storeName"></param>
        /// <returns></returns>
        public IStore GetStore(StoreConnection conn)
        {
            var builders = this.Store.GetAll();

            if ((builders != null && builders.Count > 0))
                return null;

            foreach (var each in builders)
            {
                if (each.CanHandle(conn))
                {
                    return each.GetStore(conn);
                }
            }
            return null;
        }
        #endregion
    }
}
