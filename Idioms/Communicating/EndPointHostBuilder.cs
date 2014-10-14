using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Store;
using Sandbox.Store.CoreStores;
using Sandbox.Store.Decorations.StoreOf;
using Sandbox.TypeLocation;
using ServiceStack;
using Sandbox.Extensions;
using Sandbox.TypeLocation.IoC;

namespace Sandbox.Communication
{
    /// <summary>
    /// Singleton container of host builders.  Call this to create hosts.
    /// </summary>
    public class EndPointHostBuilder
    {
        #region Constants
        public const string ENDPOINT_BUILDER_CONFIG_ID = "EndpointBuilder";
        #endregion

        #region Declarations
        private readonly object _stateLock = new object(); //the explicit object we thread lock on
        #endregion

        #region Ctor
        static EndPointHostBuilder()
        {
        }
        private EndPointHostBuilder()
        {
            lock (this._stateLock)
            {
                this.TypeContainer = new TypeContainer<IEndPointHostBuilder>();
                this.Store = new StoreOfDecoration<IEndPointHostBuilder>(new InMemoryStore());

                //add the builders to the store
                this.TypeContainer.ContainedTypes.WithEach(x =>
                {
                    IEndPointHostBuilder obj = Activator.CreateInstance(x) as IEndPointHostBuilder;
                    if (obj != null)
                        this.Store.SaveItem(obj);
                });

                //setup the default
                this.DefaultBuilder = IoCContainer.Instance.GetNamedInstance(ENDPOINT_BUILDER_CONFIG_ID) as IEndPointHostBuilder;
            }
        }
        #endregion

        #region Properties  
        public static EndPointHostBuilder Instance { get; private set; }
        private TypeContainer<IEndPointHostBuilder> TypeContainer { get; set; }
        private IStoreOf<IEndPointHostBuilder> Store { get; set; }
        private IEndPointHostBuilder DefaultBuilder { get; set; }
        #endregion

        #region Methods
        public IEndPointHostBuilder GetBuilder()
        {
            return this.DefaultBuilder;
        }
        public IEndPointHostBuilder GetBuilder(string id)
        {
            return this.Store.GetAllById<IEndPointHostBuilder>(id).FirstOrDefault();
        }
        public T GetBuilder<T>() where T:IEndPointHostBuilder
        {
            var list = this.Store.GetAll<T>();
            return list.FirstOrDefault();
        }
        #endregion
    }
}
