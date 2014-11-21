using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Idioms.Identifying;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Decoratid.Idioms.Logging
{
    /// <summary>
    /// A logger that logs to the debugger
    /// </summary>
    public class DebugLogger : ILogger
    {
        private readonly object _stateLock = new object();

        #region Ctor
        public DebugLogger()
        {
        }
        #endregion

        #region ILogger
        public void Log(int logLevel, string message, object data)
        {
            lock (this._stateLock)
            {
                Debug.WriteLine(string.Format("loglevel {0}.  message: {1}", logLevel, message));
                if (data != null)
                {
                    Debug.WriteLine(data);
                }
            }
        }
        public void Log(int logLevel, string message, object data, Exception ex)
        {
            lock (this._stateLock)
            {
                Debug.WriteLine(string.Format("loglevel {0}.  message: {1}.  error: {2}", logLevel, message, ex.StackTrace));
                if (data != null)
                {
                    Debug.WriteLine(data);
                }
            }
        }
        #endregion

        #region Static Methods
        public static DebugLogger New()
        {
            return new DebugLogger();
        }
        #endregion
    }

 
}
