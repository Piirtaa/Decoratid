using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.ValueOfing;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Logging
{
    /// <summary>
    /// Adds a logger to ValueOf evaluation
    /// </summary>
    [Serializable]
    public class LoggingValueOfDecoration<T> : DecoratedValueOfBase<T>, IHasLogger
    {
        #region Ctor
        public LoggingValueOfDecoration(IValueOf<T> decorated, ILogger logger)
            : base(decorated)
        {
            Condition.Requires(logger).IsNotNull();
            Logger = logger;
        }
        #endregion

        #region ISerializable
        protected LoggingValueOfDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public ILogger Logger { get; private set; }
        #endregion

        #region Methods
        public override T GetValue()
        {
            this.Logger.LogVerbose("GetValue started", null);

            try
            {
                return Decorated.GetValue();
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.Message, null, ex);
                throw;
            }
            finally
            {
                this.Logger.LogVerbose("GetValue completed", null);
            }
            return default(T);
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            this.Logger.LogVerbose("ApplyThisDecorationTo started", thing);

            IDecorationOf<IValueOf<T>> rv = null;
            try
            {
                rv = new LoggingValueOfDecoration<T>(thing, this.Logger);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.Message, null, ex);
                throw;
            }
            finally
            {
                this.Logger.LogVerbose("ApplyThisDecorationTo completed", null);
            }

            return rv;
        }
        #endregion
    }

    public static class LoggingValueOfDecorationExtensions
    {
        public static LoggingValueOfDecoration<T> WithLogging<T>(IValueOf<T> decorated, ILogger logger)
        {
            return new LoggingValueOfDecoration<T>(decorated, logger);
        }
    }

}
