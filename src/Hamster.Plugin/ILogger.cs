using System;

namespace Hamster.Plugin
{
    /// <summary>
    /// Interface zum abstrahieren von Logger-Funktionen,
    /// um Abhängigkeiten zum Log-Framework zu vermeiden.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Create a new child logger.  The name of the child logger is [current-loggers-name].[passed-in-name]
        /// </summary>
        /// <param name="name">The Subname of this logger.</param>
        /// <returns>The New ILogger instance.</returns>
        /// <exception cref="System.ArgumentException">If the name has an empty element name.</exception>
        ILogger CreateChildLogger( string name );

        /// <summary>
        /// Indicates if a specific level is enabled for logging.
        /// </summary>
        /// <param name="level">LogLevel to check</param>
        /// <returns>True if LogLevel is enabled</returns>
        bool IsLevelEnabled(LogLevel level);

        /// <summary>
        /// Write a message of the specified log level to the log
        /// </summary>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        void WriteMessage(LogLevel level, IFormatProvider formatProvider, string format, params object[] args);
        ///
        /// <summary>
        /// Write an exception of the specified log level to the log
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="formatProvider">The format provider to use</param>
        /// <param name="format">Format string for the message to log</param>
        /// <param name="args">Format arguments for the message to log</param>
        void WriteException(LogLevel level, Exception exception, IFormatProvider formatProvider, string format, params object[] args);
    }
}
