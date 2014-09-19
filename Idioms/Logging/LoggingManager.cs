using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Configuration;
using Decoratid.TypeLocation;
using Decoratid.Idioms.Storing;
using Decoratid.TypeLocation.IoC;

namespace Decoratid.Idioms.Logging
{
    /// <summary>
    /// Logging singleton.  Entry point to Logging subsystem
    /// </summary>
    public class LoggingManager
    {
        #region Constants
        public const string LOGGING_TYPE_CONTAINER_CONFIG_ID = "Logging";
        #endregion

        #region Declarations
        private readonly object _stateLock = new object(); //the explicit object we thread lock on
        private static LoggingManager _instance = new LoggingManager();
        #endregion

        #region Ctor
        static LoggingManager()
        {
        }
        private LoggingManager()
        {
        }
        #endregion

        #region Properties
        public static LoggingManager Instance { get { return _instance; } }
        #endregion

        #region Methods
        public ILogger GetLogger()
        {
            return IoCContainer.Instance.GetNamedInstance(LOGGING_TYPE_CONTAINER_CONFIG_ID) as ILogger;
        }
        #endregion
    }
}
