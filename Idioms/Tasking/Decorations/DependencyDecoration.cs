using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Idioms.Depending;
using System.Collections.Generic;
using System.Linq;
using Decoratid.Extensions;
using Decoratid.Core.Decorating;
using System;

namespace Decoratid.Idioms.Tasking.Decorations
{
    /// <summary>
    /// indicates the task has a dependency on other tasks in the same store
    /// </summary>
    public interface IHasTaskDependency : IDecoratedTask, IHasDependencyOf<string>
    {
    }

    /// <summary>
    /// decorate a task indicating it needs the provided tasks to complete before starting.
    /// Automatically decorates with ConditionalTriggers to accomplish this.
    /// </summary>
    public class DependencyDecoration : DecoratedTaskBase, IHasTaskDependency, IHasConditionalTaskTriggers
    {
        #region Ctor
        public DependencyDecoration(ITask decorated, List<string> prerequisiteTaskIds)
            : base(decorated.Triggered())
        {
            //^^^^ notice how we have applied the trigger decoration in the base CTOR.  this gives us conditions to add our dependency to

            //we can only have one dependency decoration per cake
            if (decorated.HasDecoration<DependencyDecoration>())
            {
                throw new InvalidOperationException("already decorated");
            }

            this.Dependency = new DependencyOf<string>(this.Id);
            prerequisiteTaskIds.WithEach(x => { this.Dependency.Prerequisites.Add(x); });

            //wire the dependency condition
            this.PerformTrigger.AppendAnd(StrategizedCondition.New((() => { return this.AreAllPrerequisiteTasksComplete(); })));
            this.CancelTrigger.AppendAnd(StrategizedCondition.New((() => { return this.AreAnyPrerequisiteTasksCancelledOrErrored(); })));

        }
        #endregion

        #region IDecoratedTask
        public override IDecorationOf<ITask> ApplyThisDecorationTo(ITask task)
        {
            return new DependencyDecoration(task, this.Dependency.Prerequisites);
        }
        #endregion

        #region Properties
        public IDependencyOf<string> Dependency { get; protected set; }
        #endregion

        #region  IHasConditionalTaskTriggers
        /// <summary>
        /// If set, defines the condition that will trigger a Perform().
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public HasCondition PerformTrigger { get { return this.As<IHasConditionalTaskTriggers>(false).PerformTrigger; } set { this.As<IHasConditionalTaskTriggers>(false).PerformTrigger = value; } }
        /// <summary>
        /// If set, defines the condition that will trigger a Cancel().
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public HasCondition CancelTrigger { get { return this.As<IHasConditionalTaskTriggers>(false).CancelTrigger; } set { this.As<IHasConditionalTaskTriggers>(false).CancelTrigger = value; } }
        /// <summary>
        /// If set, defines the condition that will trigger a MarkComplete().  Typically used to end asynchronous operations.
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public HasCondition MarkCompleteTrigger { get { return this.As<IHasConditionalTaskTriggers>(false).MarkCompleteTrigger; } set { this.As<IHasConditionalTaskTriggers>(false).MarkCompleteTrigger = value; } }
        /// <summary>
        /// If set, defines the condition that will trigger a MarkError().  Typically used to end asynchronous operations.
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public HasCondition MarkErrorTrigger { get { return this.As<IHasConditionalTaskTriggers>(false).MarkErrorTrigger; } set { this.As<IHasConditionalTaskTriggers>(false).MarkErrorTrigger = value; } }

        public void CheckTriggers() { this.As<IHasConditionalTaskTriggers>(false).CheckTriggers(); }

        #endregion



        #region Helpers
        /// <summary>
        /// for a given task, returns whether or not the prerequisites are complete
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        protected bool AreAllPrerequisiteTasksComplete()
        {
            bool isComplete = true;

            foreach (var each in this.Dependency.Prerequisites)
            {
                var depTask = this.GetTask(each);
                if (depTask.Status != DecoratidTaskStatusEnum.Complete)
                {
                    isComplete = false;
                    break;
                }
            }

            return isComplete;
        }
        /// <summary>
        /// for a given task, returns whether or not the prerequisites are complete
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        protected bool AreAnyPrerequisiteTasksCancelledOrErrored()
        {
            bool returnValue = false;

            foreach (var each in this.Dependency.Prerequisites)
            {
                var depTask = this.GetTask(each);
                if (depTask.Status == DecoratidTaskStatusEnum.Cancelled ||
                    depTask.Status == DecoratidTaskStatusEnum.Errored
                    )
                {
                    returnValue = true;
                    break;
                }
            }

            return returnValue;
        }
        #endregion
    }

    public static class DependencyDecorationExtensions
    {
        /// <summary>
        /// decorate a task indicating it needs the provided tasks to complete before starting.
        /// Automatically decorates with ConditionalTriggers to accomplish this.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <remarks>
        /// There can only be one dependency decoration per task layer cake
        /// </remarks>
        public static IHasTaskDependency DependsOn(this ITask task, params string[] prerequisiteTaskIds)
        {
            Condition.Requires(task).IsNotNull();

            //we can only have one dependency decoration per cake, so go grab that one and update it
            if (task.HasDecoration<DependencyDecoration>())
            {
                var dec = task.As<DependencyDecoration>();
                dec.Dependency.Prerequisites.AddRange(prerequisiteTaskIds);
                return dec;
            }

            return new DependencyDecoration(task, prerequisiteTaskIds.ToList());
        }
        public static ITask DoesNotDependOn(this ITask task, params string[] prerequisiteTaskIds)
        {
            Condition.Requires(task).IsNotNull();
            if (task.HasDecoration<DependencyDecoration>())
            {
                var dec = task.As<DependencyDecoration>();
                prerequisiteTaskIds.WithEach(pre =>
                {
                    dec.Dependency.Prerequisites.Remove(pre);
                });

                return dec;
            }

            return task;
        }

    }
}
