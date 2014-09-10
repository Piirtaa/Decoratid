using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Logging
{
    [Serializable]
    public class LogLevel
    {
        public const int LOGLEVEL_FATAL = 0;
        public const int LOGLEVEL_ERROR= 100;
        public const int LOGLEVEL_WARNING= 200;
        public const int LOGLEVEL_DEBUG= 300;
        public const int LOGLEVEL_INFO= 400;
        public const int LOGLEVEL_VERBOSE= 500;

    }

    public interface ILogger
    {
        void Log(int logLevel, string message,  object data);
        void Log(int logLevel, string message, object data, Exception ex);
    }

    public static class LoggerExtensions
    {
        public static void LogFatal(this ILogger logger, string message, object data)
        {
            logger.Log(LogLevel.LOGLEVEL_FATAL, message, data);
        }
        public static void LogFatal(this ILogger logger, string message, object data, Exception ex)
        {
            logger.Log(LogLevel.LOGLEVEL_FATAL, message, data, ex);
        }
        public static void LogError(this ILogger logger, string message, object data)
        {
            logger.Log(LogLevel.LOGLEVEL_ERROR, message, data);
        }
        public static void LogError(this ILogger logger, string message, object data, Exception ex)
        {
            logger.Log(LogLevel.LOGLEVEL_ERROR, message, data, ex);
        }
        public static void LogWarning(this ILogger logger, string message, object data)
        {
            logger.Log(LogLevel.LOGLEVEL_WARNING, message, data);
        }
        public static void LogWarning(this ILogger logger, string message, object data, Exception ex)
        {
            logger.Log(LogLevel.LOGLEVEL_WARNING, message, data, ex);
        }
        public static void LogDebug(this ILogger logger, string message, object data)
        {
            logger.Log(LogLevel.LOGLEVEL_DEBUG, message, data);
        }
        public static void LogDebug(this ILogger logger, string message, object data, Exception ex)
        {
            logger.Log(LogLevel.LOGLEVEL_DEBUG, message, data, ex);
        }
        public static void LogInfo(this ILogger logger, string message, object data)
        {
            logger.Log(LogLevel.LOGLEVEL_INFO, message, data);
        }
        public static void LogInfo(this ILogger logger, string message, object data, Exception ex)
        {
            logger.Log(LogLevel.LOGLEVEL_INFO, message, data, ex);
        }
        public static void LogVerbose(this ILogger logger, string message, object data)
        {
            logger.Log(LogLevel.LOGLEVEL_VERBOSE, message, data);
        }
        public static void LogVerbose(this ILogger logger, string message, object data, Exception ex)
        {
            logger.Log(LogLevel.LOGLEVEL_VERBOSE, message, data, ex);
        }
    }
}
