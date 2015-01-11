using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using System;

namespace Decoratid.Idioms.Tasking.Decorations
{
    /// <summary>
    /// Defines triggers (as ICondition) that can cause task state transitions.
    /// </summary>
    public interface IHasConditionalTaskTriggers : ITask
    {
        /// <summary>
        /// If set, defines the condition that will trigger a Perform().
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        HasCondition PerformTrigger { get; set; }
        /// <summary>
        /// If set, defines the condition that will trigger a Cancel().
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        HasCondition CancelTrigger { get; set; }
        /// <summary>
        /// If set, defines the condition that will trigger a MarkComplete().  Typically used to end asynchronous operations.
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        HasCondition MarkCompleteTrigger { get; set; }
        /// <summary>
        /// If set, defines the condition that will trigger a MarkError().  Typically used to end asynchronous operations.
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        HasCondition MarkErrorTrigger { get; set; }

        /// <summary>
        /// evaluates the trigger conditions and executes the transition, if met
        /// </summary>
        void CheckTriggers();
    }

    /// <summary>
    /// Decorates with triggers (as ICondition) for task state transitions.  Does not change the behaviour at all, just adds new behaviour
    /// in CheckTriggers that will examine the trigger conditions and perform a state transition method if the condition is true.
    /// </summary>
    [Serializable]
    public class ConditionalTriggerDecoration : DecoratedTaskBase, IHasConditionalTaskTriggers
    {
        #region Ctor
        /// <summary>
        /// ctor.  requires IStore to wrap
        /// </summary>
        /// <param name="decorated"></param>
        public ConditionalTriggerDecoration(ITask decorated)
            : base(decorated)
        {
            this.PerformTrigger = new HasCondition();
            this.CancelTrigger = new HasCondition();
            this.MarkErrorTrigger = new HasCondition();
            this.MarkCompleteTrigger = new HasCondition();
        }
        #endregion

        #region Properties
        /// <summary>
        /// If set, defines the condition that will trigger a Perform().
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public HasCondition PerformTrigger { get; set; }
        /// <summary>
        /// If set, defines the condition that will trigger a Cancel().+
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public HasCondition CancelTrigger { get; set; }
        /// <summary>
        /// If set, defines the condition that will trigger a MarkComplete().  Typically used to end asynchronous operations.
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public HasCondition MarkCompleteTrigger { get; set; }
        /// <summary>
        /// If set, defines the condition that will trigger a MarkError().  Typically used to end asynchronous operations.
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public HasCondition MarkErrorTrigger { get; set; }
        #endregion

        #region Methods
        public bool IsPerformTriggered() { return (this.PerformTrigger == null || this.PerformTrigger.Condition == null) ? false : this.PerformTrigger.Condition.Evaluate().GetValueOrDefault(); }
        public bool IsCancelTriggered() { return (this.CancelTrigger == null || this.CancelTrigger.Condition == null) ? false : this.CancelTrigger.Condition.Evaluate().GetValueOrDefault(); }
        public bool IsMarkCompleteTriggered() { return (this.MarkCompleteTrigger == null || this.MarkCompleteTrigger.Condition == null) ? false : this.MarkCompleteTrigger.Condition.Evaluate().GetValueOrDefault(); }
        public bool IsMarkErrorTriggered() { return (this.MarkErrorTrigger == null || this.MarkErrorTrigger.Condition == null) ? false : this.MarkErrorTrigger.Condition.Evaluate().GetValueOrDefault(); }

        public void CheckTriggers()
        {
            if (this.IsPerformTriggered())
            {
                this.PerformTask();
            }
            else if (this.IsMarkCompleteTriggered())
            {
                this.MarkTaskComplete();
            }
            else if (this.IsCancelTriggered())
            {
                this.CancelTask();
            }
            else if (this.IsMarkErrorTriggered())
            {
                this.MarkTaskError(new ApplicationException("Triggered Error"));
            }
        }
        #endregion

        #region IDecoratedTask
        public override IDecorationOf<ITask> ApplyThisDecorationTo(ITask task)
        {
            return new ConditionalTriggerDecoration(task);
        }
        #endregion
    }

    public static class ConditionalTriggerDecorationExtensions
    {
        /// <summary>
        /// Decorates with triggers (as ICondition) for task state transitions.  Does not change the behaviour at all, just adds new behaviour
        /// in CheckTriggers that will examine the trigger conditions and perform a state transition method if the condition is true.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static IHasConditionalTaskTriggers Triggered(this ITask task)
        {
            Condition.Requires(task).IsNotNull();

            var rv = task.As<ConditionalTriggerDecoration>();
            if (rv == null)
                rv = new ConditionalTriggerDecoration(task);

            return rv;
        }
        /// <summary>
        /// ANDs the cancel condition.  If a task doesn't have triggers, it decorates with them first.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static ITask CancelWhen(this ITask task, ICondition condition)
        {
            Condition.Requires(task).IsNotNull();
            var rTask = task.Triggered();
            rTask.CancelTrigger.AppendAnd(condition);
            return rTask;
        }
        /// <summary>
        /// tells the task to perform when the condition is true.   If a task doesn't have triggers, it decorates with them first.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static ITask PerformWhen(this ITask task, ICondition condition)
        {
            Condition.Requires(task).IsNotNull();
            var rTask = task.Triggered();
            rTask.PerformTrigger.AppendAnd(condition);
            return rTask;
        }
        /// <summary>
        /// tells the task to complete when the condition is true.   If a task doesn't have triggers, it decorates with them first.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static ITask CompleteWhen(this ITask task, ICondition condition)
        {
            Condition.Requires(task).IsNotNull();
            var rTask = task.Triggered();
            rTask.MarkCompleteTrigger.AppendAnd(condition);
            return rTask;
        }
        /// <summary>
        /// tells the task to fail when the condition is true.   If a task doesn't have triggers, it decorates with them first.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static ITask FailWhen(this ITask task, ICondition condition)
        {
            Condition.Requires(task).IsNotNull();
            var rTask = task.Triggered();
            rTask.MarkErrorTrigger.AppendAnd(condition);
            return rTask;
        }
    }
}
