using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Tasks.Decorations
{
    /// <summary>
    /// decorates with a polling background action
    /// </summary>
    public interface IPollingDecoration : IDecoratedTask
    {
        IPollingDecoration ClearBackgroundAction();
        IPollingDecoration SetBackgroundAction(ILogicOf<ITask> backgroundAction, double backgroundIntervalMSecs = 30000);
    }

    /// <summary>
    /// decorates with a polling background action
    /// </summary>
    public class PollingDecoration : DecoratedTaskBase, IPollingDecoration, IHasHydrationMap
    {
        #region Declarations
        #endregion

        #region Ctor
        public PollingDecoration(ITask decorated)
            : base(decorated)
        {
        }
        public PollingDecoration(ITask decorated, ILogicOf<ITask> backgroundAction, double backgroundIntervalMSecs = 30000)
            : base(decorated)
        {
            this.SetBackgroundAction(backgroundAction, backgroundIntervalMSecs);
        }
        #endregion

        #region Properties
        private BackgroundHost Background { get; set; }
        #endregion

        #region IDecoratedTask
        public override IDecorationOf<ITask> ApplyThisDecorationTo(ITask task)
        {
            return new PollingDecoration(task);
        }
        #endregion

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var hydrationMap = new HydrationMapValueManager<PollingDecoration>();
            hydrationMap.RegisterDefault("Background", (x) => { return x.Background; }, (x, y) => { x.Background = y as BackgroundHost; });

            return hydrationMap;
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

        #region  IPollingDecoration
        public IPollingDecoration ClearBackgroundAction()
        {
            if (this.Background != null)
            {
                this.Background.IsEnabled = false;
                try
                {
                    this.Background.Dispose();
                }
                catch { }
            }

            return this;
        }

        public IPollingDecoration SetBackgroundAction(ILogicOf<ITask> backgroundAction, double backgroundIntervalMSecs = 30000)
        {
            this.ClearBackgroundAction();
            backgroundAction.Context = (this as ITask).ValueOf();

            this.Background = new BackgroundHost(true, backgroundIntervalMSecs, backgroundAction);

            return this;
        }
        #endregion


    }
    public static partial class Extensions
    {
        /// <summary>
        /// decorates with a (null) polling action
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static IPollingDecoration DecorateWithPolling(this ITask task)
        {
            Condition.Requires(task).IsNotNull();

            if (task is IPollingDecoration)
            {
                var rTask = task as IPollingDecoration;
                return rTask;
            }

            return new PollingDecoration(task);
        }

    }
}
