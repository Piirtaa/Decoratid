using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Configuration;
using Decoratid.Extensions;
using Decoratid.Thingness.Idioms.Store;
using Decoratid.Thingness.Idioms.Store.CoreStores;

namespace Decoratid.TypeLocation.IoC
{

    public class IoCContainer
    {

        #region Declarations
        public const string PLUGIN_PREFIX = "PLUGIN_";

        private readonly object _stateLock = new object(); //the explicit object we thread lock on 
        private static IoCContainer _instance = new IoCContainer(); //the singleton instance
        #endregion

        #region Ctor
        static IoCContainer()
        {
        }
        private IoCContainer()
        {
            lock (this._stateLock)
            {
                this.Store = new InMemoryStore();

                this.AutoRegisterNamedEntries();
            }
        }
        #endregion

        #region Properties
        public static IoCContainer Instance { get { return _instance; } }
        private IStore Store { get; set; }
        #endregion

        #region Register Methods
        public void ConditionalRegisterNamedEntry(string id, object obj)
        {
            NamedEntry entry = new NamedEntry(id, obj);
            this.Store.SaveItemIfUniqueElseThrow(entry);
        }
        public void RegisterNamedEntry(string id, object obj)
        {
            NamedEntry entry = new NamedEntry(id, obj);
            this.Store.SaveItem(entry);
        }
        public void RegisterEntry(Type idType, Type registeredType)
        {
            TypedEntry entry = new TypedEntry(idType, registeredType);
            this.Store.SaveItem(entry);
        }
        /// <summary>
        /// Uses "configuration convention" and searches or all entries prefixed with "PLUGIN_" to 
        /// determine the type name associated with the plugin
        /// </summary>
        /// <param name="idType"></param>
        public void AutoRegisterNamedEntries()
        {
            var keys = ConfigStore.Instance.GetConfigKeysWithPrefix(PLUGIN_PREFIX);

            keys.WithEach(x =>
            {
                //parse out the id/name
                string id = x.Substring(6);
                var fulltypeName = ConfigStore.Instance.GetConfigEntry(x);

                var types = TypeLocator.Instance.Locate((type) =>
                {
                    return type.FullName == fulltypeName;
                });

                this.RegisterNamedEntry(id, Activator.CreateInstance(types.First()));
            });
        }
        /// <summary>
        /// Does a type resolution for types inheriting from idType and registers the first one
        /// </summary>
        /// <param name="idType"></param>
        public void AutoRegisterEntry(Type idType)
        {
            var types = TypeLocator.Instance.Locate((type) =>
            {
                return type.Equals(idType) == false 
                    && idType.IsAssignableFrom(type)
                    && type.IsAbstract == false
                    && type.GetConstructor(Type.EmptyTypes) != null;
            });

            if (types != null && types.Count > 0)
                this.RegisterEntry(idType, types.First());
        }
        public void RegisterFactoriedEntry(Type idType, Func<object> factory)
        {
            FactoriedEntry entry = new FactoriedEntry(idType, factory);
            this.Store.SaveItem(entry);
        }
        public void RegisterContextualFactoriedEntry(Type idType, Func<object, object> factory)
        {
            ContextualFactoriedEntry entry = new ContextualFactoriedEntry(idType, factory);
            this.Store.SaveItem(entry);
        }

        #endregion

        #region Broker Methods

        public object RegisterAndGetNamedEntry(string id, object obj)
        {
            NamedEntry entry = new NamedEntry(id, obj);
            this.Store.SaveItem(entry);
            return entry.GetInstance();
        }
        public object GetNamedInstance(string id)
        {
            var entry = this.Store.Get<NamedEntry>(id);
            if (entry != null)
                return entry.GetInstance();

            return null;
        }
        public T GetTypedInstance<T>()
        {
            var entry = this.Store.Get<TypedEntry>(typeof(T));
            if (entry != null)
            {
                return (T)entry.GetInstance();
            }
            return default(T);
        }
        public T GetFactoriedInstance<T>()
        {
            var entry = this.Store.Get<FactoriedEntry>(typeof(T));
            if (entry != null)
            {
                return (T)entry.GetInstance();
            }
            return default(T);
        }
        public T GetContextualFactoriedInstance<T>(object context)
        {
            var entry = this.Store.Get<ContextualFactoriedEntry>(typeof(T));
            if (entry != null)
            {
                return (T)entry.GetInstance(context);
            }
            return default(T);
        }

        /// <summary>
        /// does GetTypedInstance first, if unsuccessful GetFactoriedInstance
        /// </summary>
        /// <returns></returns>
        public T GetInstance<T>()
        {
            var instance = this.GetTypedInstance<T>();
            if (instance != null)
            {
                return instance;
            }

            instance = this.GetFactoriedInstance<T>();
            return instance;
        }
        #endregion
    }

    //public static class IoCExtensions
    //{
    //    /// <summary>
    //    /// Ioc extension hook, that will ask for an object based on 
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="source"></param>
    //    /// <returns></returns>
    //    public static T Broker<T>(this object source)
    //    {
    //        return IoCContainer.Instance.Broker<T>(source);
    //    }
    //}
}
