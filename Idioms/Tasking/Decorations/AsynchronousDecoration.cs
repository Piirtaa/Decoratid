using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;

namespace Decoratid.Idioms.Tasking.Decorations
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
    public class AsynchronousDecoration : DecoratedTaskBase, IAsynchronousDecoration
    {
        #region Ctor
        public AsynchronousDecoration(ITask decorated, ICondition markCompleteCondition, ICondition markErrorCondition)
            : base(decorated.Triggered())
        {
            Condition.Requires(markErrorCondition).IsNotNull();

            IHasConditionalTaskTriggers dec = this.FindDecoratorOf<IHasConditionalTaskTriggers>(false);
            if (markCompleteCondition != null)
                dec.ANDCompleteWhen(markCompleteCondition);
            if (markErrorCondition != null)
                dec.ANDFailWhen(markErrorCondition);

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
    }

    public static class AsynchronousDecorationExtensions
    {
        /// <summary>
        /// Decoration that flags a task as being asynchronous and provides the conditions needed to trigger completion and error.
        /// Automatically decorates with TriggerConditions.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static IAsynchronousDecoration IsAsynchronous(this ITask task, ICondition markCompleteCondition, ICondition markErrorCondition)
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
