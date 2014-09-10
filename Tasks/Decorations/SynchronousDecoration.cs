using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness;
using Decoratid.Thingness.Decorations;
using Decoratid.Thingness.Idioms.ObjectGraph;

namespace Decoratid.Tasks.Decorations
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
        public override bool Perform()
        {
            var rv = base.Perform();

            //mark completion on successful performance/error on unsuccessful
            if (rv)
                this.MarkComplete();
            else
                this.MarkError(new ApplicationException("Unsuccessful perform"));
            return rv;
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return string.Empty;
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {
        }
        #endregion
    }

    public static partial class Extensions
    {
        /// <summary>
        /// indicates the task is synchronous
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ISynchronousDecoration DecorateAsSynchronous(this ITask task)
        {
            Condition.Requires(task).IsNotNull();

            if (task is ISynchronousDecoration)
            {
                var rTask = task as ISynchronousDecoration;
                return rTask;
            }
            return new SynchronousDecoration(task);
        }
    }
}
