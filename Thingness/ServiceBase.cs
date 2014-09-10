using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Extensions;
using Decoratid.Logging;

namespace Decoratid.Thingness
{
    /// <summary>
    /// a service's state transition triggers
    /// </summary>
    public enum ServiceTriggersEnum
    {
        Initialize,
        Start,
        Stop
    }
    /// <summary>
    /// a service's states
    /// </summary>
    public enum ServiceStateEnum
    {
        Uninitialized,
        Initialized,
        Started,
        Stopped
    }
    /// <summary>
    /// interface defining service behaviour
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Implements an initialization phase, post construction.  Must be explicitly called.
        /// </summary>
        /// <returns></returns>
        bool Initialize();
        bool Start();
        bool Stop();
        ServiceStateEnum CurrentState { get; }
    }

    /// <summary>
    /// base class for services.  Has 4 states corresponding to ServiceStateEnum.  Ctor does not automatically call 
    /// Initialize().  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ServiceBase : DisposableBase, IService
    {

        #region Declarations
        private readonly object _stateLock = new object();
        private StateMachineGraph<ServiceStateEnum, ServiceTriggersEnum> _stateMachine = null;
        #endregion

        #region Ctor
        public ServiceBase()
            : this(LoggingManager.Instance.GetLogger())
        {

        }
        public ServiceBase(ILogger logger)
        {
            _stateMachine = new StateMachineGraph<ServiceStateEnum, ServiceTriggersEnum>(ServiceStateEnum.Uninitialized);
            _stateMachine.AllowTransition(ServiceStateEnum.Uninitialized, ServiceStateEnum.Initialized, ServiceTriggersEnum.Initialize);
            _stateMachine.AllowTransition(ServiceStateEnum.Initialized, ServiceStateEnum.Started, ServiceTriggersEnum.Start);
            _stateMachine.AllowTransition(ServiceStateEnum.Started, ServiceStateEnum.Stopped, ServiceTriggersEnum.Stop);
            _stateMachine.AllowTransition(ServiceStateEnum.Stopped, ServiceStateEnum.Started, ServiceTriggersEnum.Start);
            
            this.Logger = logger;
        }
        #endregion

        #region Properties
        public ILogger Logger { get; set; }
        public ServiceStateEnum CurrentState
        {
            get { return _stateMachine.CurrentState; }
        }
        #endregion

        #region Methods
        public bool Initialize()
        {
            lock (this._stateLock)
            {
                if (!_stateMachine.CanTrigger(ServiceTriggersEnum.Initialize))
                {
                    this.LogTransitionError(ServiceStateEnum.Initialized);
                    return false;
                }

                try
                {
                    this.initialize();
                    this._stateMachine.Trigger(ServiceTriggersEnum.Initialize);
                    return true;
                }
                catch (Exception ex)
                {
                    this.LogTransitionError(ServiceStateEnum.Initialized, ex);
                }
            }
            return false;
        }
        public bool Start()
        {
            lock (this._stateLock)
            {
                if (!_stateMachine.CanTrigger(ServiceTriggersEnum.Start))
                {
                    this.LogTransitionError(ServiceStateEnum.Started);
                    return false;
                }

                try
                {
                    this.start();
                    this._stateMachine.Trigger(ServiceTriggersEnum.Start);
                    return true;
                }
                catch (Exception ex)
                {
                    this.LogTransitionError(ServiceStateEnum.Started, ex);
                }
            }
            return false;
        }
        public bool Stop()
        {
            lock (this._stateLock)
            {
                if (!_stateMachine.CanTrigger(ServiceTriggersEnum.Stop))
                {
                    this.LogTransitionError(ServiceStateEnum.Stopped);
                    return false;
                }

                try
                {
                    this.initialize();
                    this._stateMachine.Trigger(ServiceTriggersEnum.Stop);
                    return true;
                }
                catch (Exception ex)
                {
                    this.LogTransitionError(ServiceStateEnum.Stopped, ex);
                }
            }
            return false;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// stops the service, then disposes it
        /// </summary>
        protected override void DisposeManaged()
        {
            this.Logger.LogDebug("disposing service", this);
            this.Stop();
            base.DisposeManaged();
        }
        #endregion

        #region Overrides
        protected virtual void initialize() { throw new NotImplementedException(); }
        protected virtual void start() { throw new NotImplementedException(); }
        protected virtual void stop() { throw new NotImplementedException(); }
        #endregion

        #region Helper Methods
        protected void LogTransitionError(ServiceStateEnum targetState)
        {
            this.Logger.LogError(string.Format("Cannot transition to {0}.", targetState), this);
        }
        protected void LogTransitionError(ServiceStateEnum targetState, Exception ex)
        {
            this.Logger.LogError(string.Format("Cannot transition to {0}.", targetState), this, ex);
        }
        #endregion
    }

}
