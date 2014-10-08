using Decoratid.Core;
using Decoratid.Idioms.StateMachining;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Serviceable
{


    /// <summary>
    /// base class for services.  Has 4 states corresponding to ServiceStateEnum.  Ctor does not automatically call 
    /// Initialize().  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ServiceBase : DisposableBase, IService, ISerializable
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

        #region ISerializable
        protected ServiceBase(SerializationInfo info, StreamingContext context)
        {
            this._stateMachine = (StateMachineGraph<ServiceStateEnum, ServiceTriggersEnum>)info.GetValue("_stateMachine", typeof(StateMachineGraph<ServiceStateEnum, ServiceTriggersEnum>));
        }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ISerializable_GetObjectData(info, context);
        }
        /// <summary>
        /// since we don't want to expose ISerializable concerns publicly, we use a virtual protected
        /// helper function that does the actual implementation of ISerializable, and is called by the
        /// explicit interface implementation of GetObjectData.  This is the method to be overridden in 
        /// derived classes.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected virtual void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_stateMachine", this._stateMachine);
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
