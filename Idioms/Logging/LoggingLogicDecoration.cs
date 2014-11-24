using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using System;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Logging
{
    /// <summary>
    /// Adds a logger to logic
    /// </summary>
    [Serializable]
    public class LoggingLogicDecoration : DecoratedLogicBase, IHasLogger
    {
        #region Ctor
        public LoggingLogicDecoration(ILogic decorated, ILogger logger)
            : base(decorated)
        {
            Condition.Requires(logger).IsNotNull();
            Logger = logger;
        }
        #endregion

        #region ISerializable
        protected LoggingLogicDecoration(SerializationInfo info, StreamingContext context)
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
        public override ILogic Perform(object context = null)
        {
            this.Logger.LogVerbose("Perform started", null);
            ILogic rv = null;
            try
            {
                rv = Decorated.Perform(context);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.Message, null, ex);
                throw;
            }
            finally
            {
                this.Logger.LogVerbose("Perform completed", null);
            }
            return rv;
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            this.Logger.LogVerbose("ApplyThisDecorationTo started", thing);

            IDecorationOf<ILogic> rv = null;

            try
            {
                rv = new LoggingLogicDecoration(thing, this.Logger);
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

    public static class LoggingLogicDecorationExtensions
    {
        public static LoggingLogicDecoration LogWith(this ILogic decorated, ILogger logger)
        {
            return new LoggingLogicDecoration(decorated, logger);
        }
    }
}
