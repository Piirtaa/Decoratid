using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Core.ValueOfing;
using Decoratid.Extensions;
using Decoratid.Idioms.Backgrounding;
using Decoratid.Idioms.Logging;
using Decoratid.Storidioms;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Backgrounding
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
    /// 
    [Serializable]
    public class PollingStoreDecoration : DecoratedStoreBase, IPollingStore//, IHasHydrationMap
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
        public PollingStoreDecoration(IStore decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected PollingStoreDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
       {
            this.BackgroundHost = (BackgroundHost)info.GetValue("BackgroundHost", typeof(BackgroundHost));
            this.BackgroundStrategy = (LogicOf<IStore>)info.GetValue("BackgroundStrategy", typeof(LogicOf<IStore>));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BackgroundHost",BackgroundHost);
            info.AddValue("BackgroundStrategy", BackgroundStrategy);
            base.ISerializable_GetObjectData(info, context);
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
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new PollingStoreDecoration(store);
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
                    backgroundAction);
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
    public static partial class PollingStoreDecorationExtensions
    {
        /// <summary>
        /// gets the first (exact type) PollDecoration 
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static PollingStoreDecoration GetPollingDecoration(this IStore decorated)
        {
            return decorated.FindDecoratorOf<PollingStoreDecoration>(true);
        }
        /// <summary>
        /// adds a background action 
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="backgroundAction"></param>
        /// <param name="backgroundIntervalMSecs"></param>
        /// <returns></returns>
        public static PollingStoreDecoration DecorateWithPolling(this IStore decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new PollingStoreDecoration(decorated);
        }

    }
}
