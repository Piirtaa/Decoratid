using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Conditional.Common;
using Decoratid.Tasks.Core;
using Decoratid.Tasks.Decorations;
using Decoratid.Core.Storing;
namespace Decoratid.Tasks
{
    public static partial  class Extensions
    {
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
