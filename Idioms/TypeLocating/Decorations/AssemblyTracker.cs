using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;
using Decoratid.Configuration;
using Decoratid.Extensions;
using Decoratid.Thingness;


namespace Decoratid.Idioms.TypeLocating
{
    /// <summary>
    /// Singleton that explicitly loads and tracks loaded assemblies.
    /// </summary>
    public class AssemblyTracker
    {
        #region Declarations
        private readonly object _stateLock = new object(); //the explicit object we thread lock on 
        private static readonly AssemblyTracker _instance = new AssemblyTracker(); //the singleton instance

        /// <summary>
        /// the prefix to use in appconfig entries that specify assembly paths to probe 
        /// </summary>
        public const string ASSEMBLY_PROBE_PATH_KEY_PREFIX = "ASSEMBLY_PROBE_PATH_";
        #endregion

        #region Ctor
        static AssemblyTracker()
        {
        }
        private AssemblyTracker()
        {
            lock (this._stateLock)
            {
                this.Assemblies = new Dictionary<string, AssemblyName>();
                this.PathsToProbeForAssemblies = new List<string>();
                this.PathsToProbeForAssemblies.Add(AppDomain.CurrentDomain.BaseDirectory);

                //wire up probe paths
                var configPaths = AppConfigUtil.FindConfigEntriesWithKeyPrefix(ASSEMBLY_PROBE_PATH_KEY_PREFIX);
                configPaths.WithEach(x =>
                {
                    if (!this.PathsToProbeForAssemblies.Contains(x.Item2) && Directory.Exists(x.Item2))
                    {
                        this.PathsToProbeForAssemblies.Add(x.Item2);
                    }
                });

                //listen to app domain events
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
                
                //refresh the list of loaded assemblies
                this.RefreshTracking();
            }
        }
        #endregion

        #region Properties
        public static AssemblyTracker Instance { get { return _instance; } }

        /// <summary>
        /// List of the loaded assemblies
        /// </summary>
        public Dictionary<string, AssemblyName> Assemblies { get; private set; }

        /// <summary>
        /// List of directories to look for assemblies
        /// </summary>
        public List<string> PathsToProbeForAssemblies { get; set; }
        #endregion

        #region Events
        public event EventHandler<EventArgOf<AssemblyName>> AssemblyRegistered;
        public void OnAssemblyRegistered(AssemblyName aName)
        {
            //skip if no listeners attached
            if (this.AssemblyRegistered == null)
                return;

            var args = this.AssemblyRegistered.BuildEventArgs(aName);
            //fire the event
            this.AssemblyRegistered(this, args);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// override the assembly resolve fallback to examine all the directories we have setup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            foreach (var each in this.PathsToProbeForAssemblies)
            {
                var di = new DirectoryInfo(each);
                var module = di.GetFiles().FirstOrDefault(i => i.Name == args.Name + ".dll");
                if (module != null)
                {
                    return Assembly.LoadFrom(module.FullName);
                }
            }
            return null;
        }

        void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            try
            {
                var assm = args.LoadedAssembly;
                var aName = assm.GetName();
                this.TrackAssembly(aName);
            }
            catch { }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Refreshes the list of assemblies tracked and returns the paths of each non-dynamic dll
        /// </summary>
        /// <returns></returns>
        public List<string> RefreshTracking()
        {
            List<string> loadedPaths = new List<string>();
            AppDomain.CurrentDomain.GetAssemblies().ToList().WithEach(x =>
            {
                //grab the path of the assembly and store it for later
                if (!x.IsDynamic)
                {
                    try
                    {
                        string loc = x.Location;
                        loadedPaths.Add(loc);
                    }
                    catch { }
                }

                //register the assembly
                try
                {
                    this.TrackAssembly(x.GetName());
                }
                catch { }
            });

            return loadedPaths;
        }


        /// <summary>
        /// explicitly loads all assemblies that are contained in the probe paths
        /// </summary>
        public void Load()
        {
            lock (this._stateLock)
            {
                //if we're a web app we force the load of all assemblies
                if (System.Web.HttpRuntime.AppDomainId != null)
                {
                    //force load (but not registration)
                    var assms = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToList();
                }

                //register/track all currently loaded assemblies
                List<string> loadedPaths = this.RefreshTracking();

                //load all assemblies that aren't currently loaded
                var allPaths = this.GetAllDllPaths();  //get all dlls in our paths
                var toLoadPaths = allPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
                toLoadPaths.WithEach(path =>
                {
                    try
                    {
                        var aName = AssemblyName.GetAssemblyName(path);
                        this.LoadAssembly(aName);
                        this.TrackAssembly(aName); //finally register
                    }
                    catch { }
                });

            }
        }
        #endregion

        #region Tracking Helpers

        /// <summary>
        /// Is the assembly tracked?
        /// </summary>
        /// <param name="aName"></param>
        /// <returns></returns>
        private bool IsAssemblyTracked(AssemblyName aName)
        {
            bool returnValue = false;

            //null check
            if (aName == null) { return returnValue; }

            //grab name - this kacks with perms and other misc. constraints 
            string name = string.Empty;
            try
            {
                name = aName.FullName;
            }
            catch
            {
                //if kacks, skip
                return returnValue;
            }

            returnValue = this.Assemblies.ContainsKey(aName.FullName);

            return returnValue;
        }

        /// <summary>
        /// Adds the assembly to the local registry of assemblies. 
        /// </summary>
        /// <param name="assm"></param>
        private void TrackAssembly(AssemblyName aName)
        {
            if (aName == null) { return; }

            if (this.IsAssemblyTracked(aName)) { return; }

            lock (this._stateLock)
            {
                if (this.IsAssemblyTracked(aName)) { return; }

                this.Assemblies.Add(aName.FullName, aName);

                try
                {
                    //raise event
                    this.OnAssemblyRegistered(aName);
                }
                catch { }
            }
        }
        #endregion

        #region Load Helpers   
        /// <summary>
        /// loads the assembly into the current app domain
        /// </summary>
        /// <param name="aName"></param>
        [DebuggerStepThrough]
        private void LoadAssembly(AssemblyName aName)
        {
            try
            {
                AppDomain.CurrentDomain.Load(aName);
                Debug.WriteLine("loading assembly: " + aName.FullName);
            }
            catch (Exception)
            {
                string s = string.Empty;
            }
        }
        /// <summary>
        /// Returns paths of all of the dlls in all of the probe paths
        /// </summary>
        /// <returns></returns>
        private List<string> GetAllDllPaths()
        {
            List<string> returnValue = new List<string>();

            foreach (string each in this.PathsToProbeForAssemblies)
            {
                var list = Directory.GetFiles(each, "*.dll", SearchOption.AllDirectories);
                if (list != null && list.Length > 0)
                {
                    returnValue.AddRange(list);
                }
            }

            return returnValue;
        }
        #endregion
    }
}
