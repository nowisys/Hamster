using System;

namespace Hamster.Plugin
{
    /// <summary>
    /// Eine Implementierung von ILogger, die nichts macht.
    /// </summary>
    public class NullLogger : ILogger
    {
        /// <summary>
        /// Instanz eines NullLogger.
        /// </summary>
        public static NullLogger Instance = new NullLogger();

        /// <summary>
        /// Erstellt eine neue Instanz von NullLogger.
        /// </summary>
        public NullLogger()
        {

        }

        public bool IsLevelEnabled(LogLevel level)
        {
            return true;
        }

        public ILogger CreateChildLogger( string name )
        {
            return this;
        }

        public void WriteMessage(LogLevel level, IFormatProvider formatProvider, string format, params object[] args)
        {
        }

        public void WriteException(LogLevel level, Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
        }
    }
}
