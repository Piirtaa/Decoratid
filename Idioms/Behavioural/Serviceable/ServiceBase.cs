using Decoratid.Idioms.StateMachineable;
using System;

namespace Decoratid.Idioms.Serviceable
{


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
        {
            _stateMachine = new StateMachineGraph<ServiceStateEnum, ServiceTriggersEnum>(ServiceStateEnum.Uninitialized);
            _stateMachine.AllowTransition(ServiceStateEnum.Uninitialized, ServiceStateEnum.Initialized, ServiceTriggersEnum.Initialize);
            _stateMachine.AllowTransition(ServiceStateEnum.Initialized, ServiceStateEnum.Started, ServiceTriggersEnum.Start);
            _stateMachine.AllowTransition(ServiceStateEnum.Started, ServiceStateEnum.Stopped, ServiceTriggersEnum.Stop);
            _stateMachine.AllowTransition(ServiceStateEnum.Stopped, ServiceStateEnum.Started, ServiceTriggersEnum.Start);
        }
        #endregion

        #region Properties
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
                    throw;
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
                    throw;
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
                    throw;
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
            this.Stop();
            base.DisposeManaged();
        }
        #endregion

        #region Overrides
        protected virtual void initialize() { throw new NotImplementedException(); }
        protected virtual void start() { throw new NotImplementedException(); }
        protected virtual void stop() { throw new NotImplementedException(); }
        #endregion

    }

}
