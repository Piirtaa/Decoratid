using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Conditions;


namespace Sandbox.Sandboxes
{
    /// <summary>
    /// A sandbox class that uses strategies to perform the actions
    /// </summary>
    public class StrategizedSandbox : SandboxBase
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public StrategizedSandbox(string name, Action init, Action start,Action stop, Func<string> getStatus):base(name)
        {
            this.InitStrategy = init;
            this.StartStrategy = start;
            this.StopStrategy = stop;
            this.GetStatusStrategy = getStatus;
        }

        #endregion

        #region Properties
        public Action InitStrategy { get; private set; }
        public Action StartStrategy { get; private set; }
        public Action StopStrategy { get; private set; }
        public Func<string> GetStatusStrategy { get; private set; }
        #endregion


        #region Overrides
        protected override void initialize()
        {
            if (this.InitStrategy != null)
            {
                this.InitStrategy();
            }
        }
        protected override void start()
        {
            if (this.StartStrategy != null)
            {
                this.StartStrategy();
            }
        }
        protected override void stop()
        {
            if (this.StopStrategy != null)
            {
                this.StopStrategy();
            }
        }
        public override string GetStatus()
        {
            if (this.GetStatusStrategy != null)
            {
                return this.GetStatusStrategy();
            }
            return string.Empty;
        }
        #endregion
    }

    /// <summary>
    /// A sandbox class that uses strategies to perform the actions
    /// </summary>
    public class ContextualStrategizedSandbox<T> : SandboxBase
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public ContextualStrategizedSandbox(string name, T context, Action<T> init, Action<T> start, Action<T> stop, Func<T,string> getStatus)
            : base(name)
        {
            this.Context = context;
            this.InitStrategy = init;
            this.StartStrategy = start;
            this.StopStrategy = stop;
            this.GetStatusStrategy = getStatus;
        }
        #endregion

        #region Properties
        public T Context { get; set; }
        public Action<T> InitStrategy { get; private set; }
        public Action<T> StartStrategy { get; private set; }
        public Action<T> StopStrategy { get; private set; }
        public Func<T,string> GetStatusStrategy { get; private set; }
        #endregion

        #region Overrides
        protected override void initialize()
        {
            if (this.InitStrategy != null)
            {
                this.InitStrategy(this.Context);
            }
        }
        protected override void start()
        {
            if (this.StartStrategy != null)
            {
                this.StartStrategy(this.Context);
            }
        }
        protected override void stop()
        {
            if (this.StopStrategy != null)
            {
                this.StopStrategy(this.Context);
            }
        }
        public override string GetStatus()
        {
            if (this.GetStatusStrategy != null)
            {
                return this.GetStatusStrategy(this.Context);
            }
            return string.Empty;
        }
        #endregion
    }
}
