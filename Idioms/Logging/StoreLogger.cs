using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Idioms.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decoratid.Idioms.Logging
{
    /// <summary>
    /// A logger that uses the provided store as the store of the log entries
    /// </summary>
    public class StoreLogger : ILogger
    {
        #region Ctor
        public StoreLogger(IStore store)
        {
            Condition.Requires(store).IsNotNull();
            this.Store = store;
            this.IdGen = new LongIdBroker("Logger", this.Store); //use the store to keep id info 
        }
        #endregion

        #region Properties
        private LongIdBroker IdGen { get; set; }
        public IStore Store { get; private set; }
        #endregion

        #region Calculated Properties
        public List<string> LogEntries
        {
            get
            {
                List<string> rv = new List<string>();
                var items = this.Store.GetAll<LogEntry>().OrderBy(x => x.Id);
                items.WithEach(x =>
                {
                    rv.Add(x.ToString());
                });
                return rv;
            }
        }
        #endregion

        #region ILogger
        public void Log(int logLevel, string message, object data)
        {
            this.Store.SaveItem(new LogEntry() { Id = this.IdGen.NextId(), Data = data, LogLevel = logLevel, Message = message });
        }
        public void Log(int logLevel, string message, object data, Exception ex)
        {
            this.Store.SaveItem(new LogEntry() { Id = this.IdGen.NextId(), Data = data, LogLevel = logLevel, Message = message, Exception = ex.StackTrace });
        }
        #endregion

        #region Static Methods
        public static StoreLogger New(IStore store)
        {
            return new StoreLogger(store);
        }
        #endregion
    }

    public class LogEntry : IHasId<Int64>
    {
        #region Ctor
        public LogEntry()
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

        #region Overrides
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        public override string ToString()
        {
            return string.Join(",", this.Id, this.UnixTime, this.LogLevel, this.Message, this.Data, this.Exception);
        }
        #endregion
    }
}
