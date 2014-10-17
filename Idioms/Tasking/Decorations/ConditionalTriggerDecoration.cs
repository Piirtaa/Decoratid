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
        ICondition PerformTrigger { get; set; }
        /// <summary>
        /// If set, defines the condition that will trigger a Cancel().
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        ICondition CancelTrigger { get; set; }
        /// <summary>
        /// If set, defines the condition that will trigger a MarkComplete().  Typically used to end asynchronous operations.
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        ICondition MarkCompleteTrigger { get; set; }
        /// <summary>
        /// If set, defines the condition that will trigger a MarkError().  Typically used to end asynchronous operations.
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        ICondition MarkErrorTrigger { get; set; }

        /// <summary>
        /// evaluates the trigger conditions and executes the transition, if met
        /// </summary>
        void CheckTriggers();
    }

    /// <summary>
    /// Decorates with triggers (as ICondition) for task state transitions.  Does not change the behaviour at all, just adds new behaviour
    /// in CheckTriggers that will examine the trigger conditions and perform a state transition method if the condition is true.
    /// </summary>
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

        }
        #endregion

        #region Properties
        /// <summary>
        /// If set, defines the condition that will trigger a Perform().
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public ICondition PerformTrigger { get; set; }
        /// <summary>
        /// If set, defines the condition that will trigger a Cancel().
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public ICondition CancelTrigger { get; set; }
        /// <summary>
        /// If set, defines the condition that will trigger a MarkComplete().  Typically used to end asynchronous operations.
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public ICondition MarkCompleteTrigger { get; set; }
        /// <summary>
        /// If set, defines the condition that will trigger a MarkError().  Typically used to end asynchronous operations.
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public ICondition MarkErrorTrigger { get; set; }
        #endregion

        #region Methods
        public  bool IsPerformTriggered() { return this.PerformTrigger == null ? false : this.PerformTrigger.Evaluate().GetValueOrDefault(); }
        public  bool IsCancelTriggered() { return this.CancelTrigger == null ? false : this.CancelTrigger.Evaluate().GetValueOrDefault(); }
        public  bool IsMarkCompleteTriggered() { return this.MarkCompleteTrigger == null ? false : this.MarkCompleteTrigger.Evaluate().GetValueOrDefault(); }
        public  bool IsMarkErrorTriggered() { return this.MarkErrorTrigger == null ? false : this.MarkErrorTrigger.Evaluate().GetValueOrDefault(); }

        public void CheckTriggers()
        {
            if (this.IsPerformTriggered())
            {
                this.Perform();
            }
            else if (this.IsMarkCompleteTriggered())
            {
                this.MarkComplete();
            }
            else if (this.IsCancelTriggered())
            {
                this.Cancel();
            }
            else if (this.IsMarkErrorTriggered())
            {
                this.MarkError(new ApplicationException("Triggered Error"));
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

            if (task is IHasConditionalTaskTriggers)
                return task as IHasConditionalTaskTriggers;

            return new ConditionalTriggerDecoration(task);
        }
        /// <summary>
        /// ANDs the cancel condition.  If a task doesn't have triggers, it decorates with them first.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static ITask ANDCancelWhen(this ITask task, ICondition condition)
        {
            Condition.Requires(task).IsNotNull();
            Condition.Requires(condition).IsNotNull();
            var rTask = task.Triggered();
            rTask.CancelTrigger = rTask.CancelTrigger.And(condition);
            return rTask;
        }
        /// <summary>
        /// tells the task to perform when the condition is true.   If a task doesn't have triggers, it decorates with them first.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static ITask ANDPerformWhen(this ITask task, ICondition condition)
        {
            Condition.Requires(task).IsNotNull();
            Condition.Requires(condition).IsNotNull();
            var rTask = task.Triggered();
            rTask.PerformTrigger = rTask.PerformTrigger.And(condition);
            return rTask;
        }
        /// <summary>
        /// tells the task to complete when the condition is true.   If a task doesn't have triggers, it decorates with them first.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static ITask ANDCompleteWhen(this ITask task, ICondition condition)
        {
            Condition.Requires(task).IsNotNull();
            Condition.Requires(condition).IsNotNull();
            var rTask = task.Triggered();
            rTask.MarkCompleteTrigger = rTask.MarkCompleteTrigger.And( condition);
            return rTask;
        }
        /// <summary>
        /// tells the task to fail when the condition is true.   If a task doesn't have triggers, it decorates with them first.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static ITask ANDFailWhen(this ITask task, ICondition condition)
        {
            Condition.Requires(task).IsNotNull();
            Condition.Requires(condition).IsNotNull();
            var rTask = task.Triggered();
            rTask.MarkErrorTrigger = rTask.MarkErrorTrigger.And( condition);
            return rTask;
        }
    }
}
