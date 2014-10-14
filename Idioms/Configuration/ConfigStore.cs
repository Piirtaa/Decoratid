using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Storing;
using Decoratid.Core.Storing;
using Decoratid.Core.Storing.Products;
using Decoratid.Thingness;
using Decoratid.Extensions;

namespace Decoratid.Configuration
{


    /// <summary>
    /// singleton container of config values
    /// </summary>
    public class ConfigStore : DisposableBase, IStore
    {
        #region Constants
        /// <summary>
        /// the default path to the config file
        /// </summary>
        public const string DEFAULT_CONFIG_FILE_PATH = "DecoratidConfig.dat";
        /// <summary>
        /// the appconfig key to a value that represents the custom location of the config file
        /// </summary>
        public const string APPCONFIG_CONFIG_FILE_PATH_KEY = "DecoratidConfig";
        #endregion

        #region Declarations
        private readonly object _stateLock = new object(); //the explicit object we thread lock on
        private static ConfigStore _instance = new ConfigStore();
        #endregion

        #region Ctor
        static ConfigStore()
        {
        }
        private ConfigStore()
        {
            //look for app config override of the config filename
            var path = this.GetBackingFileLocation();
            this.Store = new FileStore(path);

            //import any app config entries
            var keys = ConfigurationManager.AppSettings.AllKeys;

            keys.WithEach(x =>
            {
                var val = ConfigurationManager.AppSettings[x];

                this.Store.SaveItem(new ConfigEntry() { Id = x, Value = val });
            });
        }
        #endregion

        #region Properties
        public static ConfigStore Instance { get { return _instance; } }
        private IStore Store { get; set; }
        #endregion

        #region Helpers
        private string GetBackingFileLocation()
        {
            //look in the app config override first
            var path = ConfigurationManager.AppSettings[APPCONFIG_CONFIG_FILE_PATH_KEY];
            if (string.IsNullOrEmpty(path))
                path = DEFAULT_CONFIG_FILE_PATH;

            return path;
        }
        #endregion

        #region Methods
        public string GetConfigEntry(string key)
        {
            var entry = this.Store.Get<ConfigEntry>(key);
            if (entry == null)
                return null;

            return entry.Value;
        }
        public List<string> GetConfigKeys()
        {
            List<string> keys = new List<string>();
            var items = this.GetAll<ConfigEntry>();
            items.WithEach(x =>
            {
                keys.Add(x.Id);
            });
            return keys;
        }
        public List<string> GetConfigKeysWithPrefix(string prefix)
        {
            List<string> keys = new List<string>();
            var items = this.GetAll<ConfigEntry>();
            items.WithEach(x =>
            {
                if(x.Id.StartsWith(prefix))
                    keys.Add(x.Id);
            });
            return keys;
        }
        #endregion

        #region Store Methods
        public IHasId Get(IStoredObjectId soId)
        {
            return this.Store.Get(soId);
        }
        public List<T> Search<T>(SearchFilter filter) where T : IHasId
        {
            return this.Store.Search<T>(filter);
        }

        public void Commit(ICommitBag bag)
        {
            this.Store.Commit(bag);
        }

        public List<IHasId> GetAll()
        {
            return this.Store.GetAll();
        }     
        #endregion
    }
}
