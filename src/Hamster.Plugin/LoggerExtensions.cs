using System;

namespace Hamster.Plugin
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Debug(this ILogger logger, string format, params object[] args)
        {
            logger.WriteMessage(LogLevel.Debug, null, format, args);
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Debug(this ILogger logger, Exception exception, string format, params object[] args )
        {
            logger.WriteException(LogLevel.Debug, exception, null, format, args);
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Debug(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args )
        {
            logger.WriteMessage(LogLevel.Debug, formatProvider, format, args);
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Debug(this ILogger logger, Exception exception, IFormatProvider formatProvider, string format, params object[] args )
        {
            logger.WriteException(LogLevel.Debug, exception, formatProvider, format, args);
        }

        /// <summary>
        /// Logs a info message.
        /// </summary>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Info(this ILogger logger, string format, params object[] args)
        {
            logger.WriteMessage(LogLevel.Info, null, format, args);
        }

        /// <summary>
        /// Logs a info message.
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Info(this ILogger logger, Exception exception, string format, params object[] args )
        {
            logger.WriteException(LogLevel.Info, exception, null, format, args);
        }

        /// <summary>
        /// Logs a info message.
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Info(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args )
        {
            logger.WriteMessage(LogLevel.Info, formatProvider, format, args);
        }

        /// <summary>
        /// Logs a info message.
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Info(this ILogger logger, Exception exception, IFormatProvider formatProvider, string format, params object[] args )
        {
            logger.WriteException(LogLevel.Info, exception, formatProvider, format, args);
        }

        /// <summary>
        /// Logs a warn message.
        /// </summary>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Warn(this ILogger logger, string format, params object[] args)
        {
            logger.WriteMessage(LogLevel.Warn, null, format, args);
        }

        /// <summary>
        /// Logs a warn message.
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Warn(this ILogger logger, Exception exception, string format, params object[] args )
        {
            logger.WriteException(LogLevel.Warn, exception, null, format, args);
        }

        /// <summary>
        /// Logs a warn message.
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Warn(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args )
        {
            logger.WriteMessage(LogLevel.Warn, formatProvider, format, args);
        }

        /// <summary>
        /// Logs a warn message.
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Warn(this ILogger logger, Exception exception, IFormatProvider formatProvider, string format, params object[] args )
        {
            logger.WriteException(LogLevel.Warn, exception, formatProvider, format, args);
        }

        /// <summary>
        /// Logs a error message.
        /// </summary>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Error(this ILogger logger, string format, params object[] args)
        {
            logger.WriteMessage(LogLevel.Error, null, format, args);
        }

        /// <summary>
        /// Logs a error message.
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Error(this ILogger logger, Exception exception, string format, params object[] args )
        {
            logger.WriteException(LogLevel.Error, exception, null, format, args);
        }

        /// <summary>
        /// Logs a error message.
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Error(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args )
        {
            logger.WriteMessage(LogLevel.Error, formatProvider, format, args);
        }

        /// <summary>
        /// Logs a error message.
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Error(this ILogger logger, Exception exception, IFormatProvider formatProvider, string format, params object[] args )
        {
            logger.WriteException(LogLevel.Error, exception, formatProvider, format, args);
        }

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Fatal(this ILogger logger, string format, params object[] args)
        {
            logger.WriteMessage(LogLevel.Fatal, null, format, args);
        }

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Fatal(this ILogger logger, Exception exception, string format, params object[] args )
        {
            logger.WriteException(LogLevel.Fatal, exception, null, format, args);
        }

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Fatal(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args )
        {
            logger.WriteMessage(LogLevel.Fatal, formatProvider, format, args);
        }

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        public static void Fatal(this ILogger logger, Exception exception, IFormatProvider formatProvider, string format, params object[] args )
        {
            logger.WriteException(LogLevel.Fatal, exception, formatProvider, format, args);
        }
    }
}
