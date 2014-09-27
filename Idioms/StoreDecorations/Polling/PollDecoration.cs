using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness;
using Decoratid.Extensions;
using Decoratid.Logging;
using Decoratid.TypeLocation;
using Decoratid.Core.Logical;
using System.Runtime.Serialization;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Core.Storing.Decorations.Polling
{
    /// <summary>
    /// provides a background action that runs every BackgroundIntervalMSecs many milliseconds
    /// </summary>
    public interface IPollingStore : IDecoratedStore
    {
        void ClearBackgroundAction();
        void SetBackgroundAction(LogicOf<IStore> backgroundAction, double backgroundIntervalMSecs = 30000);
    }

    /// <summary>
    /// decorates, adding a polling background process to IStore, and adding ILogger
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class PollDecoration : DecoratedStoreBase, IPollingStore, IHasHydrationMap
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region ctor
        /// <summary>
        /// simple decorator.  To set background action use SetBackgroundAction
        /// </summary>
        /// <param name="decorated"></param>
        public PollDecoration(IStore decorated)
            : base(decorated)
        {
            this.Logger = LoggingManager.Instance.GetLogger();
        }
        /// <summary>
        /// detailed decorator
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="isEnabled"></param>
        /// <param name="backgroundIntervalMSecs"></param>
        /// <param name="backgroundAction"></param>
        public PollDecoration(IStore decorated
            , double backgroundIntervalMSecs, LogicOf<IStore> backgroundAction)
            : base(decorated)
        {
            this.Logger = LoggingManager.Instance.GetLogger();
            this.SetBackgroundAction(backgroundAction, backgroundIntervalMSecs);
        }
        #endregion

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var hydrationMap = new HydrationMapValueManager<PollDecoration>();
            hydrationMap.RegisterDefault("BackgroundStrategy", x => x.BackgroundStrategy, (x, y) => { x.BackgroundStrategy = y as LogicOf<IStore>; });
            hydrationMap.RegisterDefault("BackgroundHost", x => x.BackgroundHost, (x, y) => { x.BackgroundHost = y as BackgroundHost; });
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

        #region Properties
        protected LogicOf<IStore> BackgroundStrategy { get; set; }
        /// <summary>
        /// The background process container
        /// </summary>
        protected BackgroundHost BackgroundHost { get; set; }
        /// <summary>
        /// ILogger.  Injected on construction
        /// </summary>
        public ILogger Logger { get; set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new PollDecoration(store);
            if (this.BackgroundHost != null)
                returnValue.SetBackgroundAction(this.BackgroundStrategy, this.BackgroundHost.BackgroundIntervalMSecs);

            return returnValue;
        }
        #endregion

        #region IPollDecoration
        public void ClearBackgroundAction()
        {
            //clear the old task
            if (this.BackgroundHost != null)
            {
                this.BackgroundHost.Dispose();
                this.BackgroundHost = null;
            }
        }
        /// <summary>
        /// set background action 
        /// </summary>
        public void SetBackgroundAction(LogicOf<IStore> backgroundAction,
            double backgroundIntervalMSecs = 30000)
        {
            this.BackgroundStrategy = backgroundAction;

            lock (this._stateLock)
            {
                //clear the old task
                if (this.BackgroundHost != null)
                {
                    this.BackgroundHost.Dispose();
                    this.BackgroundHost = null;
                }

                backgroundAction.Context = (this as IStore).AsNaturalValue();

                this.BackgroundHost = new BackgroundHost(true, backgroundIntervalMSecs,
                    backgroundAction.DecorateWithErrorCatchingDefaultLogger());
            }
        }
        #endregion

        #region Overrides
        protected override void DisposeManaged()
        {
            if (this.BackgroundHost != null)
            {
                this.BackgroundHost.IsEnabled = false;
                this.BackgroundHost.Do(x => x.Dispose());//dispose self in a nullsafe way
            }

            base.DisposeManaged();
        }
        #endregion


    }
}
