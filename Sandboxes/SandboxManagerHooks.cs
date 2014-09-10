using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

namespace Sandbox.Sandboxes
{
    /// <summary>
    /// Container of init and quit actions to inject into the SandboxManager
    /// </summary>
    public class SandboxManagerHooks
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public SandboxManagerHooks()
        {

        }
        #endregion

        #region Properties
        public Action InitAction { get; private set; }
        public Action QuitAction { get; private set; }
        private bool HasInitialized { get; set; }
        private bool HasQuit { get; set; }
        #endregion

        #region Fluent Wire Methods
        public SandboxManagerHooks SetInit(Action action)
        {
            Condition.Requires(action).IsNotNull();
            this.InitAction = action;
            return this;
        }
        public SandboxManagerHooks SetQuit(Action action)
        {
            Condition.Requires(action).IsNotNull();
            this.QuitAction = action;
            return this;
        }
        #endregion

        #region Methods
        public void Init()
        {
            if (this.HasInitialized)
                return;

            lock (this._stateLock)
            {
                if (this.InitAction != null)
                {
                    this.InitAction();
                }
                this.HasInitialized = true;
            }
        }
        public void Quit()
        {
            if (this.HasQuit)
                return;

            lock (this._stateLock)
            {
                if (this.QuitAction != null)
                {
                    this.QuitAction();
                }
                this.HasQuit = true;
            }
        }
        #endregion
    }
}
