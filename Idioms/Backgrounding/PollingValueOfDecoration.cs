using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Logging;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Backgrounding
{
    public interface IPollingValueOf<T> : IDecoratedValueOf<T>
    {
        void ClearBackgroundAction();
        void SetBackgroundAction(LogicOf<IValueOf<T>> backgroundAction, double backgroundIntervalMSecs = 30000);
    }

    [Serializable]
    public class PollingValueOfDecoration<T> : DecoratedValueOfBase<T>, IPollingValueOf<T>
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public PollingValueOfDecoration(IValueOf<T> decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected PollingValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
       {
            this.BackgroundHost = (BackgroundHost)info.GetValue("BackgroundHost", typeof(BackgroundHost));
            this.BackgroundStrategy = (LogicOf<IValueOf<T>>)info.GetValue("BackgroundStrategy", typeof(LogicOf<IValueOf<T>>));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("BackgroundHost",BackgroundHost);
            info.AddValue("BackgroundStrategy", BackgroundStrategy);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        protected LogicOf<IValueOf<T>> BackgroundStrategy { get; set; }
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
        public void SetBackgroundAction(LogicOf<IValueOf<T>> backgroundAction,
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

                backgroundAction.Context = (this as IValueOf<T>).AsNaturalValue();

                this.BackgroundHost = new BackgroundHost(true, backgroundIntervalMSecs,
                    backgroundAction);
            }
        }
        #endregion

        #region Methods
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            var returnValue = new PollingValueOfDecoration<T>(thing);
            if (this.BackgroundHost != null)
                returnValue.SetBackgroundAction(this.BackgroundStrategy, this.BackgroundHost.BackgroundIntervalMSecs);
            return returnValue;
        }
        #endregion
    }

    public static class PollingValueOfDecorationExtensions
    {
        public static PollingValueOfDecoration<T> Polls<T>(this IValueOf<T> decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new PollingValueOfDecoration<T>(decorated);
        }
    }
}
