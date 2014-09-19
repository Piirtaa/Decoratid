using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Storing;
using Decoratid.Idioms.Storing.Products;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Logging
{
    public class StoredLogEntry : IHasId<Int64>
    {
        #region Ctor
        public StoredLogEntry()
        {
            this.UnixTime = DateTime.UtcNow.ToUnixTime();
        }
        #endregion

        #region Properties
        public Int64 UnixTime { get; set; }
        public int LogLevel { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public string Exception { get; set; }
        #endregion

        #region IHasId
        public long Id { get; set; }

        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion
    }

    /// <summary>
    /// Wrapper that exposes a logger, from a store
    /// </summary>
    public class StoreAsLogger : ILogger
    {
        #region Ctor
        public StoreAsLogger(IStore store)
        {
            Condition.Requires(store).IsNotNull();
            this.Store = store;
            this.IdGen = new UniqueIdGenerator("Logger", this.Store);
        }
        #endregion

        #region Properties
        private UniqueIdGenerator IdGen { get; set; }
        public IStore Store { get; set; }
        #endregion

        #region ILogger
        public void Log(int logLevel, string message, object data)
        {
            this.Store.SaveItem(new StoredLogEntry() { Id=this.IdGen.NextId(), Data = data, LogLevel = logLevel, Message = message });
        }

        public void Log(int logLevel, string message, object data, Exception ex)
        {
            this.Store.SaveItem(new StoredLogEntry() { Id = this.IdGen.NextId(), Data = data, LogLevel = logLevel, Message = message, Exception = ex.StackTrace });
        }
        #endregion

        #region Static Methods
        public static StoreAsLogger New(IStore store)
        {
            return new StoreAsLogger(store);
        }
        #endregion
    }
}
