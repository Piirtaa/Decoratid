using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Thingness.Idioms.Store;

using Decoratid.Thingness;
using Decoratid.Thingness.Idioms.Dependencies;
using Decoratid.Extensions;
using Decoratid.Thingness.Idioms.Store.Decorations.StoreOf;
using Decoratid.Thingness.Idioms.Store.Decorations.Evicting;
using System.Diagnostics;

namespace Decoratid.Tasks
{
    /*The design:
     *  -have a named, storeable/persistent "unit of work" (eg. argument and operation results).  thus IHasId<string> and IHasContext 
     *  -support task dependencies. thus IHasDependencyOf string.  (using names/id to define dependency)
     *  -support for async/long-running tasks.  thus the well defined state-machine
     *  -be cancellable
     *  -have trigger conditions on each state transition, for when we want the automaton to run on its own and initiate
     *      state transitions automatically.
     *  -have expiry (ie. unit of work is in expiring store).  thus the Job type/store idiom.
     *  -have throttling in terms of units of work (eg. throttled store commits).  thus store idiom 
     *  -has some event raising, so we can fire notifications
     *  
     * In other words, we want a generic task framework that will do a bunch of stuff (eg. a job) on its own, and when it
     * succeeds or fails, or is cancelled we're notified. 
     * 
     * Also we want to be able to leverage this subsystem in messaging, where a handler will package off a bunch of work to be done.

     */

    /// <summary>
    /// the task states
    /// </summary>
    public enum DecoratidTaskStatusEnum { Pending, InProcess, Cancelled, Complete, Errored }
    /// <summary>
    /// the task state transitions
    /// </summary>
    public enum DecoratidTaskTransitionEnum { Perform, MarkComplete, MarkErrored, Cancel }

    /// <summary>
    /// a task store is a store that is: storeof itask, unique id constrained, and evicting 
    /// </summary>
    public interface ITaskStore : IStoreOfUniqueId<ITask>, IEvictingStore 
    { 
    }

    /// <summary>
    /// a task.  
    /// </summary>
    /// <remarks>
    /// -IHasId -> persistence
    /// -IHasDependencyOf string -> dependency on other tasks, aggregated, uses id as proxy
    /// </remarks>
    public interface ITask : IHasId<string>, IHasSettableId<string>
    {
        //actual transition methods
        bool Perform();
        bool Cancel();
        bool MarkComplete();
        bool MarkError(Exception ex);
        DecoratidTaskStatusEnum Status { get; }
        Exception Error { get; }

        /// <summary>
        /// reference to the store of tasks this task belongs to
        /// </summary>
        ITaskStore TaskStore { get; set; }
    }

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
        public abstract bool perform();
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

        public virtual bool cancel() { return true; }
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

        public virtual bool markComplete() { return true; }
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
        public virtual bool markError(Exception ex) { return true; }
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

        #region Helpers
        protected ITask Save()
        {
            Condition.Requires(this.TaskStore).IsNotNull();
            this.TaskStore.SaveItem(this);
            return this;
        }
        protected ITask GetTask(string id)
        {
            var list = this.TaskStore.GetAllById<ITask>(id);
            return list.FirstOrDefault();
        }
        #endregion
    }
}
