using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Core.ValueOfing;
using Decoratid.Extensions;
using Decoratid.Idioms.Backgrounding;
using Decoratid.Idioms.Logging;
using Decoratid.Storidioms.Logging;

namespace Decoratid.Storidioms.Polling
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
    public class PollDecoration : DecoratedStoreBase, IPollingStore//, IHasHydrationMap
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region ctor
        /// <summary>
        /// detailed decorator
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="isEnabled"></param>
        /// <param name="backgroundIntervalMSecs"></param>
        /// <param name="backgroundAction"></param>
        public PollDecoration(IStore decorated, ILogger logger)
            : base(decorated.LogWith(logger))
        {
        }
        #endregion

        //#region IHasHydrationMap
        //public virtual IHydrationMap GetHydrationMap()
        //{
        //    var hydrationMap = new HydrationMapValueManager<PollDecoration>();
        //    hydrationMap.RegisterDefault("BackgroundStrategy", x => x.BackgroundStrategy, (x, y) => { x.BackgroundStrategy = y as LogicOf<IStore>; });
        //    hydrationMap.RegisterDefault("BackgroundHost", x => x.BackgroundHost, (x, y) => { x.BackgroundHost = y as BackgroundHost; });
        //    return hydrationMap;
        //}
        //#endregion

        //#region IDecorationHydrateable
        //public override string DehydrateDecoration(IGraph uow = null)
        //{
        //    return this.GetHydrationMap().DehydrateValue(this, uow);
        //}
        //public override void HydrateDecoration(string text, IGraph uow = null)
        //{
        //    this.GetHydrationMap().HydrateValue(this, text, uow);
        //}
        //#endregion

        #region Properties
        protected LogicOf<IStore> BackgroundStrategy { get; set; }
        /// <summary>
        /// The background process container
        /// </summary>
        private BackgroundHost BackgroundHost { get; set; }
        /// <summary>
        /// ILogger.  Injected on construction
        /// </summary>
        private ILogger Logger { get { return this.FindDecoratorOf<ILoggingStore>(false).Logger; } }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new PollDecoration(store, this.Logger);
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
                    backgroundAction.LogWith(this.Logger));
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

    /// <summary>
    /// Fluent decorator.  By convention put all FluentDecorators in Decoratid.Core.Storing namespace.
    /// </summary>
    public static partial class FluentDecorator
    {
        /// <summary>
        /// gets the first (exact type) PollDecoration 
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static PollDecoration GetPollingDecoration(this IStore decorated)
        {
            return decorated.FindDecoratorOf<PollDecoration>(true);
        }
        /// <summary>
        /// adds a background action 
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="backgroundAction"></param>
        /// <param name="backgroundIntervalMSecs"></param>
        /// <returns></returns>
        public static PollDecoration DecorateWithPolling(this IStore decorated, ILogger logger)
        {
            Condition.Requires(decorated).IsNotNull();
            return new PollDecoration(decorated, logger);
        }

    }
}
