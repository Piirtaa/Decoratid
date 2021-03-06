﻿using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using System;

namespace Decoratid.Idioms.Tasking.Decorations
{
    /// <summary>
    /// indicates the task is synchronous. marker interface.  
    /// </summary>
    public interface ISynchronousDecoration : IDecoratedTask
    {
    }

    /// <summary>
    /// indicates the task is synchronous
    /// </summary>
    public class SynchronousDecoration : DecoratedTaskBase, ISynchronousDecoration
    {
        #region Ctor
        public SynchronousDecoration(ITask decorated)
            : base(decorated)
        {

        }
        #endregion

        #region Properties
        public ITask InterruptingTask { get; private set; }
        #endregion

        #region IDecoratedTask
        public override IDecorationOf<ITask> ApplyThisDecorationTo(ITask task)
        {
            return new SynchronousDecoration(task);
        }
        #endregion

        #region Overrides
        public override bool PerformTask()
        {
            var rv = base.PerformTask();

            //mark completion on successful performance/error on unsuccessful
            if (rv)
                this.MarkTaskComplete();
            else
                this.MarkTaskError(new ApplicationException("Unsuccessful perform"));
            return rv;
        }
        #endregion
    }

    public static class SynchronousDecorationExtensions
    {
        /// <summary>
        /// indicates the task is synchronous
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ISynchronousDecoration IsSynchronous(this ITask task)
        {
            Condition.Requires(task).IsNotNull();
            var rv =  task.As<SynchronousDecoration>();
            if (rv == null)
                rv = new SynchronousDecoration(task);
            return rv;
        }
    }
}
