using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Idioms.Logging;
using System;
using System.Runtime.Serialization;
using Decoratid.Core.ValueOfing;

namespace Decoratid.Idioms.Backgrounding
{

    public interface IPollingLogic : IDecoratedLogic
    {
        void ClearBackgroundAction();
        void SetBackgroundAction(LogicOf<ILogic> backgroundAction, double backgroundIntervalMSecs = 30000);
    }

    [Serializable]
    public class PollingLogicDecoration : DecoratedLogicBase, IPollingLogic
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public PollingLogicDecoration(ILogic decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected PollingLogicDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.BackgroundHost = (BackgroundHost)info.GetValue("BackgroundHost", typeof(BackgroundHost));
            this.BackgroundStrategy = (LogicOf<ILogic>)info.GetValue("BackgroundStrategy", typeof(LogicOf<ILogic>));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BackgroundHost", BackgroundHost);
            info.AddValue("BackgroundStrategy", BackgroundStrategy);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        protected LogicOf<ILogic> BackgroundStrategy { get; set; }
        /// <summary>
        /// The background process container
        /// </summary>
        private BackgroundHost BackgroundHost { get; set; }

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
        public void SetBackgroundAction(LogicOf<ILogic> backgroundAction,
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

                backgroundAction.Context = (this as ILogic).AsNaturalValue();

                this.BackgroundHost = new BackgroundHost(true, backgroundIntervalMSecs,
                    backgroundAction);
            }
        }
        #endregion

        #region Methods
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            var returnValue = new PollingLogicDecoration(thing);
            if (this.BackgroundHost != null)
                returnValue.SetBackgroundAction(this.BackgroundStrategy, this.BackgroundHost.BackgroundIntervalMSecs);
            return returnValue;
        }
        #endregion
    }

    public static class PollingLogicDecorationExtensions
    {
        public static PollingLogicDecoration Polls(this ILogic decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new PollingLogicDecoration(decorated);
        }
    }
}
