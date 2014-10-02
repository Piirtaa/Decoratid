using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Conditions;
using Sandbox.Conditions.Common;
using Sandbox.Configuration;
using Sandbox.Extensions;
using Sandbox.Reflection;
using Sandbox.Store;
using Sandbox.Thingness;

namespace Decoratid.Idioms.TypeLocating
{
    /// <summary>
    /// Container of a single type instance of the generic type.  Resolves the instance with the provided config key - this will
    /// point to the FullName of the type we will instantiate.  If that type is unavail, an error will be thrown.  Config store entry we are looking
    /// for is ConfigEntry
    /// </summary>
    public class PluginContainer<T> : DisposableBase, IHasContext<T>
    {
        #region Declarations
        private readonly object _stateLock = new object();
        private TypeContainer<T> _types = null;
        #endregion

        #region Ctor
        public PluginContainer(string key) :base()
        {
            Condition.Requires(key).IsNotNullOrEmpty();
            this.Key = key;

            //load types
            this._types = new TypeContainer<T>();

            var entry = ConfigStore.Instance.GetById<ConfigEntry>(this.Key);
            Condition.Requires(entry).IsNotNull("Invalid Plugin : " + this.Key);

            Type type = this._types.ContainedTypes.Find(x => x.FullName == entry.Value);
            Condition.Requires(type).IsNotNull("Invalid Plugin TypeName : " + entry.Value);

            var instance = Activator.CreateInstance(type);
            this.Context = (T)instance;
        }
        /// <summary>
        /// creates the container explicitly
        /// </summary>
        /// <param name="key"></param>
        /// <param name="instance"></param>
        public PluginContainer(string key, T instance) :base()
        {
            Condition.Requires(key).IsNotNullOrEmpty();
            this.Key = key;
            this.Context = instance;
        }
        #endregion

        #region Properties
        public string Key { get; private set; }
        public T Context { get; set; }
        #endregion

        #region Overrides
        protected override void DisposeManaged()
        {
            if (this.Context is IDisposable)
            {
                ((IDisposable)(this.Context)).Dispose();
            }
            base.DisposeManaged();
        }
        #endregion

        #region Save To Config
        //saves the current instance
        public void SaveToConfig()
        {
            ConfigStore.Instance.SaveItem(new ConfigEntry(){ Id=this.Key, Value = this.Context.GetType().FullName});
        }
        #endregion
    }
}
