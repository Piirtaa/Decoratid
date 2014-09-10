using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Tasks.Core;
using Decoratid.Thingness;
using Decoratid.Thingness.Decorations;
using Decoratid.Thingness.Idioms.ObjectGraph.Values;
using Decoratid.Thingness.Idioms.ObjectGraph;

namespace Decoratid.Tasks.Decorations
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
    public class ConditionalTriggerDecoration : DecoratedTaskBase, IHasConditionalTaskTriggers, IHasHydrationMap
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

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var map = new HydrationMapValueManager<ConditionalTriggerDecoration>();
            map.RegisterDefault("PerformTrigger", x => x.PerformTrigger, (x, y) => { x.PerformTrigger = y as ICondition; });
            map.RegisterDefault("CancelTrigger", x => x.CancelTrigger, (x, y) => { x.CancelTrigger = y as ICondition; });
            map.RegisterDefault("MarkCompleteTrigger", x => x.MarkCompleteTrigger, (x, y) => { x.MarkCompleteTrigger = y as ICondition; });
            map.RegisterDefault("MarkErrorTrigger", x => x.MarkErrorTrigger, (x, y) => { x.MarkErrorTrigger = y as ICondition; });
            return map;
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return this.GetHydrationMap().DehydrateValue(this, uow);
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {
            this.GetHydrationMap().HydrateValue(this, text, uow);
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

    public static partial class Extensions
    {
        /// <summary>
        /// Decorates with triggers (as ICondition) for task state transitions.  Does not change the behaviour at all, just adds new behaviour
        /// in CheckTriggers that will examine the trigger conditions and perform a state transition method if the condition is true.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static IHasConditionalTaskTriggers DecorateWithTriggerConditions(this ITask task)
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
        public static ITask ANDCancelCondition(this ITask task, ICondition condition)
        {
            Condition.Requires(task).IsNotNull();
            Condition.Requires(condition).IsNotNull();
            var rTask = task.DecorateWithTriggerConditions();
            rTask.CancelTrigger = rTask.CancelTrigger.And(condition);
            return rTask;
        }
        /// <summary>
        /// tells the task to perform when the condition is true.   If a task doesn't have triggers, it decorates with them first.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static ITask ANDPerformCondition(this ITask task, ICondition condition)
        {
            Condition.Requires(task).IsNotNull();
            Condition.Requires(condition).IsNotNull();
            var rTask = task.DecorateWithTriggerConditions();
            rTask.PerformTrigger = rTask.PerformTrigger.And(condition);
            return rTask;
        }
        /// <summary>
        /// tells the task to complete when the condition is true.   If a task doesn't have triggers, it decorates with them first.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static ITask ANDCompleteCondition(this ITask task, ICondition condition)
        {
            Condition.Requires(task).IsNotNull();
            Condition.Requires(condition).IsNotNull();
            var rTask = task.DecorateWithTriggerConditions();
            rTask.MarkCompleteTrigger = rTask.MarkCompleteTrigger.And( condition);
            return rTask;
        }
        /// <summary>
        /// tells the task to fail when the condition is true.   If a task doesn't have triggers, it decorates with them first.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static ITask ANDFailCondition(this ITask task, ICondition condition)
        {
            Condition.Requires(task).IsNotNull();
            Condition.Requires(condition).IsNotNull();
            var rTask = task.DecorateWithTriggerConditions();
            rTask.MarkErrorTrigger = rTask.MarkErrorTrigger.And( condition);
            return rTask;
        }
    }
}
