using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Dependencies;
using Decoratid.Extensions;
using Decoratid.Tasks.Core;
using Decoratid.Core.Conditional;
using Decoratid.Thingness;
using Decoratid.Core.Logical;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Tasks.Decorations
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
    public class DependencyDecoration : DecoratedTaskBase, IHasTaskDependency, IHasConditionalTaskTriggers, IHasHydrationMap
    {
        #region Ctor
        public DependencyDecoration(ITask decorated, List<string> prerequisiteTaskIds)
            : base(decorated.DecorateWithTriggerConditions())
        {
            //^^^^ notice how we have applied the trigger decoration in the base CTOR.  this gives us conditions to add our dependency to

            this.Dependency = new DependencyOf<string>(this.Id);
            prerequisiteTaskIds.WithEach(x => { this.Dependency.Prerequisites.Add(x); });

            //wire the dependency condition
            this.PerformTrigger = this.PerformTrigger.And(StrategizedCondition.New((() => { return this.AreAllPrerequisiteTasksComplete(); })));
            this.CancelTrigger = this.CancelTrigger.And(StrategizedCondition.New((() => { return this.AreAnyPrerequisiteTasksCancelledOrErrored(); })));

        }
        #endregion

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var map = new HydrationMapValueManager<DependencyDecoration>();
            map.RegisterDefault("Dependency", x => x.Dependency, (x, y) => { x.Dependency = y as IDependencyOf<string>; });
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
        public ICondition PerformTrigger { get { return this.FindDecoratorOf<IHasConditionalTaskTriggers>(false).PerformTrigger; } set { this.FindDecoratorOf<IHasConditionalTaskTriggers>(false).PerformTrigger = value; } }
        /// <summary>
        /// If set, defines the condition that will trigger a Cancel().
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public ICondition CancelTrigger { get { return this.FindDecoratorOf<IHasConditionalTaskTriggers>(false).CancelTrigger; } set { this.FindDecoratorOf<IHasConditionalTaskTriggers>(false).CancelTrigger = value; } }
        /// <summary>
        /// If set, defines the condition that will trigger a MarkComplete().  Typically used to end asynchronous operations.
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public ICondition MarkCompleteTrigger { get { return this.FindDecoratorOf<IHasConditionalTaskTriggers>(false).MarkCompleteTrigger; } set { this.FindDecoratorOf<IHasConditionalTaskTriggers>(false).MarkCompleteTrigger = value; } }
        /// <summary>
        /// If set, defines the condition that will trigger a MarkError().  Typically used to end asynchronous operations.
        /// </summary>
        /// <remarks>Must be property settable (not just ctor settable) to enable the condition to have references to the task itself</remarks>
        public ICondition MarkErrorTrigger { get { return this.FindDecoratorOf<IHasConditionalTaskTriggers>(false).MarkErrorTrigger; } set { this.FindDecoratorOf<IHasConditionalTaskTriggers>(false).MarkErrorTrigger = value; } }

        public void CheckTriggers() { this.FindDecoratorOf<IHasConditionalTaskTriggers>(false).CheckTriggers(); }

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

    public static partial class Extensions
    {
        /// <summary>
        /// decorate a task indicating it needs the provided tasks to complete before starting.
        /// Automatically decorates with ConditionalTriggers to accomplish this.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static IHasTaskDependency DecorateWithDependency(this ITask task, params string[] prerequisiteTaskIds)
        {
            Condition.Requires(task).IsNotNull();

            if (task is IHasTaskDependency)
            {
                var rTask = task as IHasTaskDependency;
                rTask.Dependency.Prerequisites.AddRange(prerequisiteTaskIds);
                return rTask;
            }

            return new DependencyDecoration(task, prerequisiteTaskIds.ToList());
        }
        /// <summary>
        /// Removes the outermost IHasTaskDependency decoration and returns the decorated value.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="prerequisiteTaskIds"></param>
        /// <returns></returns>
        public static ITask RemoveDependencyDecoration(this IHasTaskDependency task)
        {
            Condition.Requires(task).IsNotNull();

            //wrap task
            return task.Decorated;
        }

    }
}
