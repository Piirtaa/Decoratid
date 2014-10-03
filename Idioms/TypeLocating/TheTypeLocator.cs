using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Utils;


namespace Decoratid.Idioms.TypeLocating
{

    /// <summary>
    /// Singleton container of ITypeLocator
    /// </summary>
    public class TheTypeLocator 
    {
        #region Declarations
        private readonly object _stateLock = new object(); //the explicit object we thread lock on 
        private static TheTypeLocator _instance = new TheTypeLocator(); //the singleton instance
        #endregion

        #region Ctor
        static TheTypeLocator()
        {
        }
        private TheTypeLocator()
        {
            lock (this._stateLock)
            {
                //by default use the default type locator
                this.Locator = NaturalTypeLocator.New().TrackAssemblies().CacheTypes();
            }
        }
        #endregion

        #region Properties
        public static TheTypeLocator Instance { get { return _instance; } }
        public ITypeLocator Locator { get; set; }
        #endregion


        //#region Helper
        ///// <summary>
        ///// Loads the type locator, by first examining configuration for an explicit plugin, and if not found, it uses the default
        ///// </summary>
        //private void LoadLocator()
        //{
        //    var typeLocatorType = ConfigurationManager.AppSettings[TYPE_LOCATOR_KEY];
        //    this.Locator = ReflectionUtil.CreateObject(typeLocatorType) as ITypeLocator;

            
        //    if (this.Locator == null) { this.Locator = new DefaultTypeLocator(); }
        //}
        //#endregion


    }
}
