using CuttingEdge.Conditions;
using Decoratid.Core;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Idioms.StateMachining;
using System;
using System.Diagnostics;
using System.Linq;

namespace Decoratid.Idioms.Tasking
{

    /// <summary>
    /// Abstract template implementation of ITask
    /// </summary>
    [Serializable]
    public abstract class TaskBase : DisposableBase, ITask
    {
        #region Declarations
        protected readonly object _stateLock = new object();
        protected StateMachineGraph<DecoratidTaskStatusEnum, DecoratidTaskTransitionEnum> _stateMachine = null;
        #endregion

        #region Ctor
        [DebuggerStepThrough]
        public TaskBase(string id)
        {
            Condition.Requires(id).IsNotNullOrEmpty();

            //define the graph
            _stateMachine = new StateMachineGraph<DecoratidTaskStatusEnum, DecoratidTaskTransitionEnum>(DecoratidTaskStatusEnum.Pending);
            _stateMachine.AllowTransition(DecoratidTaskStatusEnum.Pending, DecoratidTaskStatusEnum.InProcess, DecoratidTaskTransitionEnum.Perform);
            _stateMachine.AllowTransition(DecoratidTaskStatusEnum.Pending, DecoratidTaskStatusEnum.Cancelled, DecoratidTaskTransitionEnum.Cancel);
            _stateMachine.AllowTransition(DecoratidTaskStatusEnum.InProcess, DecoratidTaskStatusEnum.Complete, DecoratidTaskTransitionEnum.MarkComplete);
            _stateMachine.AllowTransition(DecoratidTaskStatusEnum.InProcess, DecoratidTaskStatusEnum.Errored, DecoratidTaskTransitionEnum.MarkErrored);
            _stateMachine.AllowTransition(DecoratidTaskStatusEnum.InProcess, DecoratidTaskStatusEnum.Cancelled, DecoratidTaskTransitionEnum.Cancel);

            this.Id = id;
        }
        #endregion

        #region ITask Properties
        public DecoratidTaskStatusEnum Status { get { return this._stateMachine.CurrentState; } }
        public string Id { get; private set; }
        object IHasId.Id { get { return this.Id; } }
        public ITaskStore TaskStore { get; set; }
        public Exception Error { get; protected set; }
        public void SetId(string id) { this.Id = id; }
        void SetId(object id) { this.SetId(id as string); }

        #endregion

        #region ITask Methods
        protected abstract bool perform();
        public bool Perform()
        {
            bool returnValue = false;

            lock (this._stateLock)
            {
                if (this._stateMachine.Trigger(DecoratidTaskTransitionEnum.Perform))
                {
                    try
                    {
                        returnValue = this.perform();
                    }
                    catch (Exception ex)
                    {
                        this.MarkError(ex);
                    }
                    finally
                    {
                        if (returnValue)
                            this.Save();
                    }
                }
            }
            return returnValue;
        }

        protected virtual bool cancel() { return true; }
        public bool Cancel()
        {
            bool returnValue = false;

            lock (this._stateLock)
            {
                if (this._stateMachine.Trigger(DecoratidTaskTransitionEnum.Cancel))
                {
                    try
                    {
                        returnValue = this.cancel();
                    }
                    catch (Exception ex)
                    {
                        this.MarkError(ex);
                    }
                    finally
                    {
                        if (returnValue)
                            this.Save();
                    }
                }
            }
            return returnValue;
        }

        protected virtual bool markComplete() { return true; }
        public bool MarkComplete()
        {
            bool returnValue = false;

            lock (this._stateLock)
            {
                if (this._stateMachine.Trigger(DecoratidTaskTransitionEnum.MarkComplete))
                {
                    try
                    {
                        returnValue = this.markComplete();
                    }
                    catch (Exception ex)
                    {
                        this.MarkError(ex);
                    }
                    finally
                    {
                        if (returnValue)
                            this.Save();
                    }
                }
            }
            return returnValue;
        }
        protected virtual bool markError(Exception ex) { return true; }
        public bool MarkError(Exception ex)
        {
            bool returnValue = false;

            lock (this._stateLock)
            {
                if (this._stateMachine.Trigger(DecoratidTaskTransitionEnum.MarkErrored))
                {
                    try
                    {
                        returnValue = this.markError(ex);
                    }
                    catch (Exception ex2)
                    {
                        this.MarkError(ex2);
                    }
                    finally
                    {
                        if (returnValue)
                            this.Save();
                    }
                }
            }
            return returnValue;
        }
        #endregion
    }
}
