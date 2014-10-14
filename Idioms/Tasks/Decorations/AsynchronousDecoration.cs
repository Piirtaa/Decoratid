using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Thingness;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;

namespace Decoratid.Tasks.Decorations
{
    /// <summary>
    /// flags a task as being asynchronous
    /// </summary>
    public interface IAsynchronousDecoration : IDecoratedTask
    {
    }

    /// <summary>
    /// Decoration that flags a task as being asynchronous and provides the conditions needed to trigger completion and error.
    /// Automatically decorates with TriggerConditions.
    /// </summary>
    public class AsynchronousDecoration : DecoratedTaskBase, IAsynchronousDecoration, IHasHydrationMap
    {
        #region Ctor
        public AsynchronousDecoration(ITask decorated, ICondition markCompleteCondition, ICondition markErrorCondition)
            : base(decorated.DecorateWithTriggerConditions())
        {
            Condition.Requires(markErrorCondition).IsNotNull();

            IHasConditionalTaskTriggers dec = this.FindDecoratorOf<IHasConditionalTaskTriggers>(false);
            if (markCompleteCondition != null)
                dec.ANDCompleteCondition(markCompleteCondition);
            if (markErrorCondition != null)
                dec.ANDFailCondition(markErrorCondition);

            //set placeholders so the decoration can be cloned via ApplyThisDecorationTo
            //we can't pull the conditions from the decoration as the decorated task may itself have existing conditions
            this.MarkCompleteCondition = markCompleteCondition;
            this.MarkErrorCondition = markErrorCondition;
        }
        #endregion

        #region Properties
        /// <summary>
        /// placeholder
        /// </summary>
        private ICondition MarkCompleteCondition { get; set; }
        /// <summary>
        /// placeholder
        /// </summary>
        private ICondition MarkErrorCondition { get; set; }
        #endregion

        #region IDecoratedTask
        public override IDecorationOf<ITask> ApplyThisDecorationTo(ITask task)
        {
            return new AsynchronousDecoration(task, this.MarkCompleteCondition, this.MarkErrorCondition);
        }
        #endregion

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var map = new HydrationMapValueManager<AsynchronousDecoration>();
            map.RegisterDefault("MarkCompleteCondition", x => x.MarkCompleteCondition, (x, y) => { x.MarkCompleteCondition = y as ICondition; });
            map.RegisterDefault("MarkErrorCondition", x => x.MarkErrorCondition, (x, y) => { x.MarkErrorCondition = y as ICondition; });
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
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Decoration that flags a task as being asynchronous and provides the conditions needed to trigger completion and error.
        /// Automatically decorates with TriggerConditions.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static IAsynchronousDecoration DecorateAsAsynchronous(this ITask task, ICondition markCompleteCondition, ICondition markErrorCondition)
        {
            Condition.Requires(task).IsNotNull();

            if (task is IAsynchronousDecoration)
            {
                var rTask = task as IAsynchronousDecoration;
                return rTask;
            }
            return new AsynchronousDecoration(task, markCompleteCondition, markErrorCondition);
        }
    }
}
