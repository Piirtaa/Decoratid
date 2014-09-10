using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Reflection;


namespace Decoratid.TypeLocation
{
    /// <summary>
    /// ITypeLocator searches for types based on a delegate filter. 
    /// </summary>
    public interface ITypeLocator
    {
        /// <summary>
        /// Returns all types that return bool from the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        List<Type> Locate(Func<Type, bool> filter);
    }

    /// <summary>
    /// Singleton container of ITypeLocator
    /// </summary>
    public class TypeLocator : ITypeLocator
    {
        #region Constants
        /// <summary>
        /// the config key that contains the pluggable type locator type name
        /// </summary>
        public const string TYPE_LOCATOR_KEY = "TYPE_LOCATOR";
        #endregion

        #region Declarations
        private readonly object _stateLock = new object(); //the explicit object we thread lock on 
        private static TypeLocator _instance = new TypeLocator(); //the singleton instance
        #endregion

        #region Ctor
        static TypeLocator()
        {
        }
        private TypeLocator()
        {
            lock (this._stateLock)
            {
                this.LoadLocator();
            }
        }
        #endregion

        #region Properties
        public static TypeLocator Instance { get { return _instance; } }
        private ITypeLocator Locator { get; set; }
        #endregion

        #region Methods
        public List<Type> Locate(Func<Type, bool> filter)
        {
            List<Type> returnValue = new List<Type>();

            var list = this.Locator.Locate(filter);
            if(list != null)
                returnValue.AddRange(list );

            return returnValue;
        }
        #endregion

        #region Helper
        /// <summary>
        /// Loads the type locator, by first examining configuration for an explicit plugin, and if not found, it uses the default
        /// </summary>
        private void LoadLocator()
        {
            var typeLocatorType = ConfigurationManager.AppSettings[TYPE_LOCATOR_KEY];
            this.Locator = ReflectionUtil.CreateObject(typeLocatorType) as ITypeLocator;

            
            if (this.Locator == null) { this.Locator = new DefaultTypeLocator(); }
        }
        #endregion


    }
}
