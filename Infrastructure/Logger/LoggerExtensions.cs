using System;
using System.Linq;
using AiDollar.Infrastructure.Logger;

namespace System
{
    public static class LoggerExtensions
    {
        public static void InjectLogger(this object obj, ILogger logger)
        {
            if (obj == null)
                return;

            var prop = (
                from p in obj.GetType().GetProperties()
                where p.PropertyType == typeof(ILogger) &&
                      p.CanWrite
                select p).FirstOrDefault();

            prop?.SetValue(obj, logger);
        }
    }
}

namespace AiDollar.Infrastructure.Logger
{
    public static class LoggerExtensions
    {
        public static void LogDebug(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Debug, message);
        }

        public static void LogDebug(this ILogger logger, string format, params object[] args)
        {
            logger.Log(LogLevel.Debug, string.Format(format, args));
        }

        public static void LogInformation(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Information, message);
        }

        public static void LogInformation(this ILogger logger, string format, params object[] args)
        {
            logger.Log(LogLevel.Information, string.Format(format, args));
        }

        public static void LogWarning(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Warning, message);
        }

        public static void LogWarning(this ILogger logger, string format, params object[] args)
        {
            logger.Log(LogLevel.Warning, string.Format(format, args));
        }

        public static void LogWarning(this ILogger logger, string message, Exception error)
        {
            logger.Log(LogLevel.Warning, message, error);
        }

        public static void LogError(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Error, message);
        }

        public static void LogError(this ILogger logger, string format, params object[] args)
        {

            logger.Log(LogLevel.Error, string.Format(format, args));
        }

        public static void LogError(this ILogger logger, string message, Exception error)
        {
            logger.Log(LogLevel.Error, message, error);
        }
    }
}
