using System;

namespace Hamster.Plugin
{
    /// <summary>
    /// Repräsentiert eine geloggte Nachricht.
    /// </summary>
    [Serializable()]
    public class LogEventArgs : EventArgs
    {
        private int id;
        private string logName;
        private string message;
        private string exceptionText;
        private DateTime time;
        private string level;

        /// <summary>
        /// Erstellt eine neue Instanz von LogEventArgs.
        /// </summary>
        /// <param name="id">Eindeutiger Identifikator der Nachricht.</param>
        /// <param name="logName">Name unter dem die Nachricht gelogt wird.</param>
        /// <param name="message">Inhalt der Nachricht.</param>
        /// <param name="time">Zeitpunkt an dem die Nachricht aufgetreten ist.</param>
        /// <param name="level">Bezeichnung des Levels der Nachricht (DEBUG,INFO,WARN,ERROR,FATAL)</param>
        public LogEventArgs(int id, DateTime time, string level, string logName, string message, string exceptionText)
        {
            this.id = id;
            this.logName = logName;
            this.message = message;
            this.exceptionText = exceptionText;
            this.time = time;
            this.level = level;
        }

        /// <summary>
        /// Zeigt den eindeutigen Identifikator der Nachricht.
        /// </summary>
        public int Id
        {
            get { return id; }
        }

        /// <summary>
        /// Zeigt den Name unter dem die Nachricht gelogt wird.
        /// </summary>
        public string LogName
        {
            get { return logName; }
        }

        /// <summary>
        /// Zeigt den Inhalt der Nachricht.
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// Zeigt die Textrepräsentation der geloggten Exception.
        /// </summary>
        public string ExceptionText
        {
            get { return exceptionText; }
        }

        /// <summary>
        /// Zeigt den Zeitpunkt an dem die Nachricht aufgetreten ist.
        /// </summary>
        public DateTime Time
        {
            get { return time; }
        }

        /// <summary>
        /// Bezeichnung des Levels der Nachricht (DEBUG,INFO,WARN,ERROR,FATAL)
        /// </summary>
        public string Level
        {
            get { return level; }
        }
    }
}
