﻿using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Idioms.Tasking.Decorations;
using System.Linq;
using Decoratid.Core.Storing;

namespace Decoratid.Idioms.Tasking
{
    public static partial  class Extensions
    {
        #region IHasConditionalTaskTrigger Get Set Helpers
        /// <summary>
        /// returns the specified trigger condition on the provided task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public static ICondition GetTriggerCondition(this IHasConditionalTaskTriggers task, DecoratidTaskTransitionEnum transition)
        {
            //apply the operation
            ICondition condition = null;

            switch (transition)
            {
                case DecoratidTaskTransitionEnum.Cancel:
                    condition = task.CancelTrigger;
                    break;
                case DecoratidTaskTransitionEnum.MarkComplete:
                    condition = task.MarkCompleteTrigger;
                    break;
                case DecoratidTaskTransitionEnum.MarkErrored:
                    condition = task.MarkErrorTrigger;
                    break;
                case DecoratidTaskTransitionEnum.Perform:
                    condition = task.PerformTrigger;
                    break;
            }
            return condition;
        }

        /// <summary>
        /// sets the specified trigger condition on the provided task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public static void SetTriggerCondition(this IHasConditionalTaskTriggers task, DecoratidTaskTransitionEnum transition, ICondition condition)
        {
            switch (transition)
            {
                case DecoratidTaskTransitionEnum.Cancel:
                    task.CancelTrigger = condition;
                    break;
                case DecoratidTaskTransitionEnum.MarkComplete:
                    task.MarkCompleteTrigger = condition;
                    break;
                case DecoratidTaskTransitionEnum.MarkErrored:
                    task.MarkErrorTrigger = condition;
                    break;
                case DecoratidTaskTransitionEnum.Perform:
                    task.PerformTrigger = condition;
                    break;
            }
        }
        #endregion

        #region Boolean Operations on task Trigger Conditions
        /// <summary>
        /// performs AND operation on the specified trigger condition
        /// </summary>
        /// <param name="task"></param>
        /// <param name="transition"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IHasConditionalTaskTriggers AndTriggerCondition(this IHasConditionalTaskTriggers task, DecoratidTaskTransitionEnum transition,
    ICondition condition)
        {
            task.SetTriggerCondition(transition, task.GetTriggerCondition(transition).And(condition));
            return task;
        }
        /// <summary>
        /// performs De-AND operation on the specified trigger condition
        /// </summary>
        /// <param name="task"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public static IHasConditionalTaskTriggers DeAndTriggerCondition(this IHasConditionalTaskTriggers task, DecoratidTaskTransitionEnum transition)
        {
            task.SetTriggerCondition(transition, task.GetTriggerCondition(transition).DeAnd().FirstOrDefault());
            return task;
        }
        /// <summary>
        /// performs OR operation on the specified trigger condition
        /// </summary>
        /// <param name="task"></param>
        /// <param name="transition"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IHasConditionalTaskTriggers OrTriggerCondition(this IHasConditionalTaskTriggers task, DecoratidTaskTransitionEnum transition,
            ICondition condition)
        {
            task.SetTriggerCondition(transition, task.GetTriggerCondition(transition).Or(condition));
            return task;
        }
        /// <summary>
        /// performs De-OR operation on the specified trigger condition
        /// </summary>
        /// <param name="task"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public static IHasConditionalTaskTriggers DeOrTriggerCondition(this IHasConditionalTaskTriggers task, DecoratidTaskTransitionEnum transition)
        {
            task.SetTriggerCondition(transition, task.GetTriggerCondition(transition).DeOr().FirstOrDefault());
            return task;
        }
        #endregion

        /// <summary>
        /// saves the task in the task's task store
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ITask Save(this ITask task)
        {
            Condition.Requires(task).IsNotNull();
            Condition.Requires(task.TaskStore).IsNotNull();
            task.TaskStore.SaveItem(task);
            return task;
        }
        public static ITask GetTask(this ITask task, string id)
        {
            var list = task.TaskStore.GetAllById<ITask>(id);

            return list.FirstOrDefault();
        }




    }
}