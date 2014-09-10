using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Configuration;
using Sandbox.Thingness;
using Sandbox.Thingness.Dependencies.Named;
using Sandbox.TypeLocation;
using Sandbox.Extensions;
using Sandbox.Store;

namespace Sandbox.Sandboxes
{
    /// <summary>
    /// singleton manager of ISandboxes.  This guy lives for the life of the App, and autowires Sandbox instances by
    /// looking for all ISandbox types in the AppDomain. Hooks exist for injection of behaviour on (one-time) Start and Dispose,
    /// allowing for custom ISandbox to be added programmatically.  Has methods to explictly start/stop sandboxes 
    /// (and their dependencies).
    /// </summary>
    /// <remarks>
    /// Usage:
    /// SandboxManager.Instance.Hooks = ...
    /// using(SandboxManager.Instance)
    /// {
    ///     SandboxManager.Instance.Start();
    /// 
    ///     //wait for explicit close event
    ///     //dispose will shut everything down
    /// }
    /// 
    /// </remarks>
    public class SandboxManager : ServiceBase
    {
        #region Constants
        private const string SANDBOX_CONFIG_KEY = "Sandbox";
        private const string DEPENDENCY_CLASS = "Sandbox";

        #endregion

        #region Declarations
        private readonly object _stateLock = new object(); //the explicit object we thread lock on 
        private static SandboxManager _instance = new SandboxManager(); //the singleton instance

        #endregion

        #region Ctor
        static SandboxManager()
        {
        }
        private SandboxManager()
            : base()
        {
            this.Initialize();
        }
        #endregion

        #region Properties
        public static SandboxManager Instance { get; private set; }
        /// <summary>
        /// The container of Sandboxes and their dependency info
        /// </summary>
        private NamedDependencySet Set { get; set; }
        /// <summary>
        /// injectable 1-time behaviour to occur on start and dispose
        /// </summary>
        public SandboxManagerHooks Hooks { get; set; }
        #endregion

        #region Calculated Properties
        /// <summary>
        /// returns the sandbox names of each sandbox
        /// </summary>
        public List<string> SandboxNames
        {
            get
            {
                return this.Sandboxes.Select(x => x.Item1.Name).ToList();
            }
        }
        /// <summary>
        /// returns the dependency Self names of each sandbox
        /// </summary>
        public List<string> SandboxDependencyNames
        {
            get
            {
                return this.Sandboxes.Select(x => x.Item2.With(o=>o.Self)).ToList();
            }
        }
        /// <summary>
        /// returns all sandboxes, sorted by dependency
        /// </summary>
        public List<Tuple<ISandbox,NamedDependency>> Sandboxes
        {
            get
            {
                var list= this.Set.GetTotalDependencyOrder();
                List<Tuple<ISandbox, NamedDependency>> returnValue = new List<Tuple<ISandbox, NamedDependency>>();
                list.WithEach(x =>
                {
                    returnValue.Add(new Tuple<ISandbox,NamedDependency>(x.Instance as ISandbox, x));
                });
                return returnValue;
            }
        }
        #endregion

        #region Single Sandbox Methods
        /// <summary>
        /// given a sandbox Name, returns its dependency Self property
        /// </summary>
        /// <param name="sandboxName"></param>
        /// <returns></returns>
        public string GetDependencyName(string sandboxName)
        {
            var item = this.Set.GetTotalDependencyOrder().SingleOrDefault(x => x.GetInstance<ISandbox>().Name == sandboxName);
            if (item != null)
            {
                return item.Self;
            }
            return null;
        }
        protected bool PerformOnSandboxTree(string dependencyName, Action<ISandbox> action, Func<ISandbox, bool> passCondition, bool reverse)
        {
            //get dependency
            var item = this.Set.GetDependencyItem(dependencyName);
            Condition.Requires(item).IsNotNull();

            var tree = this.Set.GetDependencyTree(item);
            tree.Add(item);

            if (reverse)
            {
                tree.Reverse();
            }

            //perform action on all deps in sequence 
            tree.WithEach(dep =>
            {
                ISandbox sandbox = dep.Instance as ISandbox;
                if (sandbox != null)
                {
                    action(sandbox);
                }
            });

            //check condition on all deps
            bool allTrue = tree.All(dep =>
            {
                ISandbox sandbox = dep.Instance as ISandbox;
                if (sandbox != null)
                {
                    return passCondition(sandbox);
                }
                return true;
            });

            return allTrue;
        }
        /// <summary>
        /// using the dependency name, initializes the dependency tree
        /// </summary>
        /// <param name="dependencyName"></param>
        /// <returns></returns>
        public bool InitializeSandbox(string dependencyName)
        {
            return this.PerformOnSandboxTree(dependencyName, (x) => x.Initialize(), (x) =>
            {
                return x.CurrentState != ServiceStateEnum.Uninitialized;
            }, false);
        }
        /// <summary>
        /// using the dependency name, starts the dependency tree
        /// </summary>
        /// <param name="dependencyName"></param>
        /// <returns></returns>
        public bool StartSandbox(string dependencyName)
        {
            return this.PerformOnSandboxTree(dependencyName, 
            (x) => 
            { 
                x.Initialize();
                x.Start();
            },
            (x) =>
            {
                return x.CurrentState == ServiceStateEnum.Started;
            }, false);
        }
        /// <summary>
        /// stops the single dependency
        /// </summary>
        /// <param name="dependencyName"></param>
        /// <returns></returns>
        public bool StopSandbox(string dependencyName)
        {
            var item = this.Set.GetDependencyItem(dependencyName);
            Condition.Requires(item).IsNotNull();

            ISandbox sandbox = item.Instance as ISandbox;
            if (sandbox != null)
            {
                return sandbox.Stop();
            }
            return true;
        }
        /// <summary>
        /// using the dependency name, stops the dependency tree
        /// </summary>
        /// <param name="dependencyName"></param>
        /// <returns></returns>
        public bool StopSandboxTree(string dependencyName)
        {
            return this.PerformOnSandboxTree(dependencyName,
            (x) =>
            {
                x.Stop();
            },
            (x) =>
            {
                return x.CurrentState == ServiceStateEnum.Stopped;
            }, true);
        }
        #endregion

        #region Registration Methods
        /// <summary>
        /// add a sandbox instance manually
        /// </summary>
        /// <param name="sandbox"></param>
        public void RegisterService(ISandbox sandbox, NamedDependency dep)
        {
            Condition.Requires(sandbox).IsNotNull();

            lock (this._stateLock)
            {
                this.Set.AddDependency(sandbox, dep);
            }
        }
        #endregion

        #region overrides
        /// <summary>
        /// initialize the sandboxes
        /// </summary>
        protected override void initialize()
        {
            //get all the sandbox types we have configured
            TypeContainerConfig config = ConfigStore.Instance.GetById<TypeContainerConfig>(SANDBOX_CONFIG_KEY);
            var container = new TypeContainer<ISandbox>(config);

            this.Set = NamedDependencySet.BuildSetFromTypeAttributes(container.RegisteredTypes, DEPENDENCY_CLASS);

            var deps = this.Set.GetTotalDependencyOrder();

            //initialize
            deps.WithEach(x =>
            {
                ISandbox sandbox = x.Instance as ISandbox;
                sandbox.Initialize();
            });

        }
        /// <summary>
        /// starts all sandboxes
        /// </summary>
        protected override void start()
        {
            //run the injectable behaviour
            if (this.Hooks != null)
            {
                this.Hooks.Init();
            }

            var deps = this.Set.GetTotalDependencyOrder();

            //start
            deps.WithEach(x =>
            {
                ISandbox sandbox = x.Instance as ISandbox;
                sandbox.Initialize();
                sandbox.Start();
            });
        }
        /// <summary>
        /// stops all sandboxes
        /// </summary>
        protected override void stop()
        {
            var deps = this.Set.GetTotalDependencyOrder();
            deps.Reverse();

            //initialize
            deps.WithEach(x =>
            {
                ISandbox sandbox = x.Instance as ISandbox;
                sandbox.Stop();
            });
        }
        protected override void DisposeManaged()
        {
            this.Stop();
            
            //run the injectable behaviour
            if (this.Hooks != null)
            {
                this.Hooks.Quit();
            }

            base.DisposeManaged();
        }
        #endregion



    }
}
