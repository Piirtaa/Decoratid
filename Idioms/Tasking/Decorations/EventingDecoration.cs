using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Extensions;
using System;
using System.Collections.Generic;

namespace Decoratid.Idioms.Tasking.Decorations
{
    /// <summary>
    /// task decoration that fires an event when a task state transition happens.  Keeps a list of actions to
    /// perform on task state transitions.
    /// </summary>
    public interface IEventingTask : IDecoratedTask
    {
        event EventHandler<EventArgOf<ITask>> TaskStarted;

        event EventHandler<EventArgOf<ITask>> TaskCancelled;

        event EventHandler<EventArgOf<ITask>> TaskCompleted;

        event EventHandler<EventArgOf<ITask>> TaskErrored;

        //fluent event subscription hooks
        IEventingTask DoOnTaskStarted(Action action);
        IEventingTask DoOnTaskCancelled(Action action);
        IEventingTask DoOnTaskCompleted(Action action);
        IEventingTask DoOnTaskErrored(Action action);
    }

    //TODO: add hydration map and serialize the task subscriptions??.  change from action to ilogic??

    /// <summary>
    /// decorates with task events.  fires an event when a task state transition happens.  Keeps a list of actions to
    /// perform on task state transitions.
    /// </summary>
    public class EventingDecoration : DecoratedTaskBase, IEventingTask
    {
        #region Ctor
        public EventingDecoration(ITask decorated)
            : base(decorated)
        {
            //add event handlers
            this.TaskStarted += EventingDecoration_TaskStarted;
            this.TaskCancelled += EventingDecoration_TaskCancelled;
            this.TaskCompleted += EventingDecoration_TaskCompleted;
            this.TaskErrored += EventingDecoration_TaskErrored;

            this.OnTaskCancelledActions = new List<Action>();
            this.OnTaskCompletedActions = new List<Action>();
            this.OnTaskErroredActions = new List<Action>();
            this.OnTaskStartedActions = new List<Action>();
        }
        #endregion

        #region Properties
        private List<Action> OnTaskStartedActions { get; set; }
        private List<Action> OnTaskCancelledActions { get; set; }
        private List<Action> OnTaskCompletedActions { get; set; }
        private List<Action> OnTaskErroredActions { get; set; }
        #endregion

        #region Overrides
        public override bool MarkTaskComplete()
        {
            var rv = base.MarkTaskComplete();
            if (rv)
                this.TaskCompleted.BuildAndFireEventArgs<ITask>(this);
            return rv;
        }
        public override bool MarkTaskError(Exception ex)
        {
            var rv = base.MarkTaskError(ex);
            if (rv)
                this.TaskErrored.BuildAndFireEventArgs<ITask>(this);
            return rv;
        }
        public override bool CancelTask()
        {
            var rv = base.CancelTask();
            if (rv)
                this.TaskCancelled.BuildAndFireEventArgs<ITask>(this);
            return rv;
        }
        public override bool PerformTask()
        {
            var rv = base.PerformTask();
            if (rv)
                this.TaskStarted.BuildAndFireEventArgs<ITask>(this);
            return rv;
        }
        #endregion

        #region IEventingTask
        public event EventHandler<EventArgOf<ITask>> TaskStarted;

        public event EventHandler<EventArgOf<ITask>> TaskCancelled;

        public event EventHandler<EventArgOf<ITask>> TaskCompleted;

        public event EventHandler<EventArgOf<ITask>> TaskErrored;

        //fluent event subscription hooks
        public IEventingTask DoOnTaskStarted(Action action)
        {
            this.OnTaskStartedActions.Add(action);
            return this;
        }
        public IEventingTask DoOnTaskCancelled(Action action)
        {
            this.OnTaskCancelledActions.Add(action);
            return this;
        }
        public IEventingTask DoOnTaskCompleted(Action action)
        {
            this.OnTaskCompletedActions.Add(action);
            return this;
        }
        public IEventingTask DoOnTaskErrored(Action action)
        {
            this.OnTaskErroredActions.Add(action);
            return this;
        }
        #endregion

        #region Event Handlers
        void EventingDecoration_TaskErrored(object sender, EventArgOf<ITask> e)
        {
            this.OnTaskErroredActions.WithEach(x => { x(); });
        }

        void EventingDecoration_TaskCompleted(object sender, EventArgOf<ITask> e)
        {
            this.OnTaskCompletedActions.WithEach(x => { x(); });
        }

        void EventingDecoration_TaskCancelled(object sender, EventArgOf<ITask> e)
        {
            this.OnTaskCancelledActions.WithEach(x => { x(); });
        }

        void EventingDecoration_TaskStarted(object sender, EventArgOf<ITask> e)
        {
            this.OnTaskStartedActions.WithEach(x => { x(); });
        }
        #endregion

        #region IDecoratedTask
        public override IDecorationOf<ITask> ApplyThisDecorationTo(ITask task)
        {
            return new EventingDecoration(task);
        }
        #endregion
    }

    public static class EventingDecorationExtensions
    {
        /// <summary>
        /// decorates with task events.  fires an event when a task state transition happens.  Keeps a list of actions to
        /// perform on task state transitions.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static IEventingTask Eventing(this ITask task)
        {
            Condition.Requires(task).IsNotNull();
            return new EventingDecoration(task);
        }

    }
}
