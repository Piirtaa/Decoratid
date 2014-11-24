using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Idioms.StateMachining;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Tasking
{
    /// <summary>
    /// decorates logic with tasking
    /// </summary>
    public class TaskingLogicDecoration : DecoratedLogicBase, ITask
    {
        #region Declarations
        protected readonly object _stateLock = new object();
        protected StateMachineGraph<DecoratidTaskStatusEnum, DecoratidTaskTransitionEnum> _stateMachine = null;
        #endregion

        #region Ctor
        public TaskingLogicDecoration(ILogic decorated, string taskId, ILogic cancelLogic = null)
            : base(decorated)
        {
            Condition.Requires(taskId).IsNotNullOrEmpty();
            this.Id = taskId;

            //define the graph
            _stateMachine = new StateMachineGraph<DecoratidTaskStatusEnum, DecoratidTaskTransitionEnum>(DecoratidTaskStatusEnum.Pending);
            _stateMachine.AllowTransition(DecoratidTaskStatusEnum.Pending, DecoratidTaskStatusEnum.InProcess, DecoratidTaskTransitionEnum.Perform);
            _stateMachine.AllowTransition(DecoratidTaskStatusEnum.Pending, DecoratidTaskStatusEnum.Cancelled, DecoratidTaskTransitionEnum.Cancel);
            _stateMachine.AllowTransition(DecoratidTaskStatusEnum.InProcess, DecoratidTaskStatusEnum.Complete, DecoratidTaskTransitionEnum.MarkComplete);
            _stateMachine.AllowTransition(DecoratidTaskStatusEnum.InProcess, DecoratidTaskStatusEnum.Errored, DecoratidTaskTransitionEnum.MarkErrored);
            _stateMachine.AllowTransition(DecoratidTaskStatusEnum.InProcess, DecoratidTaskStatusEnum.Cancelled, DecoratidTaskTransitionEnum.Cancel);

            this.CancelLogic = cancelLogic;
        }
        #endregion

        #region Properties
        public DecoratidTaskStatusEnum Status { get { return this._stateMachine.CurrentState; } }
        public string Id { get; private set; }
        object IHasId.Id { get { return this.Id; } }
        public ITaskStore TaskStore { get; set; }
        public Exception Error { get; protected set; }
        private ILogic CancelLogic { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// the logic decoration.  invokes the task perform
        /// </summary>
        public override ILogic Perform(object context = null)
        {
            var b = this.PerformTask();
            return this;
        }
        /// <summary>
        /// the task perform
        /// </summary>
        /// <returns></returns>
        public bool PerformTask()
        {
            bool returnValue = false;

            lock (this._stateLock)
            {
                if (this._stateMachine.Trigger(DecoratidTaskTransitionEnum.Perform))
                {
                    try
                    {
                        this.Decorated.Perform();
                        returnValue = true;
                    }
                    catch (Exception ex)
                    {
                        this.MarkTaskError(ex);
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
        public bool CancelTask()
        {
            bool returnValue = false;

            lock (this._stateLock)
            {
                if (this._stateMachine.Trigger(DecoratidTaskTransitionEnum.Cancel))
                {
                    try
                    {
                        if (this.CancelLogic != null)
                            this.CancelLogic.Perform();

                        returnValue = true;
                    }
                    catch (Exception ex)
                    {
                        this.MarkTaskError(ex);
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
        public bool MarkTaskComplete()
        {
            bool returnValue = false;

            lock (this._stateLock)
            {
                if (this._stateMachine.Trigger(DecoratidTaskTransitionEnum.MarkComplete))
                {
                    this.Save();
                }
            }
            return returnValue;
        }
        public bool MarkTaskError(Exception ex)
        {
            bool returnValue = false;

            lock (this._stateLock)
            {
                if (this._stateMachine.Trigger(DecoratidTaskTransitionEnum.MarkErrored))
                {
                    this.Save();
                }
            }
            return returnValue;
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new TaskingLogicDecoration(thing, this.Id);
        }
        #endregion
    }

    public static class TaskingLogicDecorationExtensions
    {
        public static TaskingLogicDecoration Tasking(ILogic decorated, string taskId, ILogic cancelLogic = null)
        {
            Condition.Requires(decorated).IsNotNull();
            return new TaskingLogicDecoration(decorated, taskId, cancelLogic);
        }

    }
}
