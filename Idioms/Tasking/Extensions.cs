using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Idioms.Tasking.Decorations;
using System.Linq;
using Decoratid.Core.Storing;
using Decoratid.Core.Logical;
using Decoratid.Extensions;
using System;

namespace Decoratid.Idioms.Tasking
{
    public static partial class Extensions
    {
    //    #region IHasConditionalTaskTrigger Get Set Helpers
    //    /// <summary>
    //    /// returns the specified trigger condition on the provided task
    //    /// </summary>
    //    /// <param name="task"></param>
    //    /// <param name="transition"></param>
    //    /// <returns></returns>
    //    public static ICondition GetTriggerCondition(this IHasConditionalTaskTriggers task, DecoratidTaskTransitionEnum transition)
    //    {
    //        //apply the operation
    //        ICondition condition = null;

    //        switch (transition)
    //        {
    //            case DecoratidTaskTransitionEnum.Cancel:
    //                condition = task.CancelTrigger.Condition;
    //                break;
    //            case DecoratidTaskTransitionEnum.MarkComplete:
    //                condition = task.MarkCompleteTrigger.Condition;
    //                break;
    //            case DecoratidTaskTransitionEnum.MarkErrored:
    //                condition = task.MarkErrorTrigger.Condition;
    //                break;
    //            case DecoratidTaskTransitionEnum.Perform:
    //                condition = task.PerformTrigger.Condition;
    //                break;
    //        }
    //        return condition;
    //    }

    //    /// <summary>
    //    /// sets the specified trigger condition on the provided task
    //    /// </summary>
    //    /// <param name="task"></param>
    //    /// <param name="transition"></param>
    //    /// <returns></returns>
    //    public static void SetTriggerCondition(this IHasConditionalTaskTriggers task, DecoratidTaskTransitionEnum transition, ICondition condition)
    //    {
    //        switch (transition)
    //        {
    //            case DecoratidTaskTransitionEnum.Cancel:
    //                task.CancelTrigger.AppendAnd(condition);
    //                break;
    //            case DecoratidTaskTransitionEnum.MarkComplete:
    //                task.MarkCompleteTrigger.AppendAnd(condition);
    //                break;
    //            case DecoratidTaskTransitionEnum.MarkErrored:
    //                task.MarkErrorTrigger.AppendAnd(condition);
    //                break;
    //            case DecoratidTaskTransitionEnum.Perform:
    //                task.PerformTrigger.AppendAnd(condition);
    //                break;
    //        }
    //    }
    //    #endregion

    //    #region Boolean Operations on task Trigger Conditions
    //    /// <summary>
    //    /// performs AND operation on the specified trigger condition
    //    /// </summary>
    //    /// <param name="task"></param>
    //    /// <param name="transition"></param>
    //    /// <param name="condition"></param>
    //    /// <returns></returns>
    //    public static IHasConditionalTaskTriggers AndTriggerCondition(this IHasConditionalTaskTriggers task, DecoratidTaskTransitionEnum transition,
    //ICondition condition)
    //    {
    //        task.SetTriggerCondition(transition, task.GetTriggerCondition(transition).And(condition));
    //        return task;
    //    }
    //    /// <summary>
    //    /// performs De-AND operation on the specified trigger condition
    //    /// </summary>
    //    /// <param name="task"></param>
    //    /// <param name="transition"></param>
    //    /// <returns></returns>
    //    public static IHasConditionalTaskTriggers DeAndTriggerCondition(this IHasConditionalTaskTriggers task, DecoratidTaskTransitionEnum transition)
    //    {
    //        task.SetTriggerCondition(transition, task.GetTriggerCondition(transition).DeAnd().FirstOrDefault());
    //        return task;
    //    }
    //    /// <summary>
    //    /// performs OR operation on the specified trigger condition
    //    /// </summary>
    //    /// <param name="task"></param>
    //    /// <param name="transition"></param>
    //    /// <param name="condition"></param>
    //    /// <returns></returns>
    //    public static IHasConditionalTaskTriggers OrTriggerCondition(this IHasConditionalTaskTriggers task, DecoratidTaskTransitionEnum transition,
    //        ICondition condition)
    //    {
    //        task.SetTriggerCondition(transition, task.GetTriggerCondition(transition).Or(condition));
    //        return task;
    //    }
    //    /// <summary>
    //    /// performs De-OR operation on the specified trigger condition
    //    /// </summary>
    //    /// <param name="task"></param>
    //    /// <param name="transition"></param>
    //    /// <returns></returns>
    //    public static IHasConditionalTaskTriggers DeOrTriggerCondition(this IHasConditionalTaskTriggers task, DecoratidTaskTransitionEnum transition)
    //    {
    //        task.SetTriggerCondition(transition, task.GetTriggerCondition(transition).DeOr().FirstOrDefault());
    //        return task;
    //    }
    //    #endregion

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
        /// <summary>
        /// given a task, examines the task's store to find the specified task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ITask GetTask(this ITask task, string id)
        {
            var list = task.TaskStore.GetAllById<ITask>(id);

            return list.FirstOrDefault();
        }

        #region Fluent
        /// <summary>
        /// converts logic into a task
        /// </summary>
        /// <param name="logic"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ITask MakeTask(this ILogic logic, string id)
        {
            Condition.Requires(logic).IsNotNull();

            if (logic.IsOfLogic())
            {
                var ofType = logic.GetOfType();
                var ofTaskType = typeof(StrategizedTaskOf<>);
                var genType = ofTaskType.MakeGenericType(ofType);
                var ofTask = Activator.CreateInstance(genType, id, null, logic, null);
                return ofTask as ITask;
            }
            else
            {
                var task = StrategizedTask.New(id, logic);
                return task;
            }
        }
        #endregion


    }
}
