using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Logging;
using Decoratid.Storidioms;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Logging
{
    /// <summary>
    /// provides a background action that runs every BackgroundIntervalMSecs many milliseconds
    /// </summary>
    public interface ILoggingStore : IDecoratedStore, IHasLogger
    {
    }

    [Serializable]
    public class LoggingStoreDecoration : DecoratedStoreBase, ILoggingStore//, IHasHydrationMap
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region ctor
        /// <summary>
        /// simple decorator.  To set background action use SetBackgroundAction
        /// </summary>
        /// <param name="decorated"></param>
        public LoggingStoreDecoration(IStore decorated, ILogger logger)
            : base(decorated)
        {
            Condition.Requires(logger).IsNotNull();
            this.Logger = logger;
        }
        #endregion

        #region ISerializable
        protected LoggingStoreDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public ILogger Logger { get; set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new LoggingStoreDecoration(store, this.Logger);
            return returnValue;
        }
        #endregion

        #region Overrides
        public override void Commit(ICommitBag bag)
        {
            this.Logger.LogVerbose("Commit started", null);

            try
            {
                Decorated.Commit(bag);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.Message, null, ex);
                throw;
            }
            finally
            {
                this.Logger.LogVerbose("Commit completed", null);
            }
        }
        public override Core.Identifying.IHasId Get(IStoredObjectId soId)
        {
            this.Logger.LogVerbose("Get started", null);

            try
            {
                return Decorated.Get(soId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.Message, null, ex);
                throw;
            }
            finally
            {
                this.Logger.LogVerbose("Get completed", null);
            }
        }
        public override List<IHasId> GetAll()
        {
            this.Logger.LogVerbose("GetAll started", null);

            try
            {
                return Decorated.GetAll();
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.Message, null, ex);
                throw;
            }
            finally
            {
                this.Logger.LogVerbose("GetAll completed", null);
            }
        }
        public override List<T> Search<T>(SearchFilter filter)
        {
            this.Logger.LogVerbose("Search started", null);

            try
            {
                return Decorated.Search<T>(filter);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.Message, null, ex);
                throw;
            }
            finally
            {
                this.Logger.LogVerbose("Search completed", null);
            }
        }
        #endregion


    }

    public static class LoggingStoreDecorationExtensions
    {
        public static LoggingStoreDecoration LogWith(this IStore decorated, ILogger logger)
        {
            Condition.Requires(decorated).IsNotNull();
            return new LoggingStoreDecoration(decorated, logger);
        }

        public static LoggingStoreDecoration GetLogger(this IStore decorated)
        {
            return decorated.FindDecoratorOf<LoggingStoreDecoration>(true);
        }

    }
}
