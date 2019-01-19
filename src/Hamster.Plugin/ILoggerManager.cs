using System;
using System.Collections.Generic;

namespace Hamster.Plugin
{
    /// <summary>
    /// Interface zum Zugriff auf eine zentrale Logverwaltung.
    /// </summary>
    public interface ILoggerManager
    {
        /// <summary>
        /// Wird ausgelöst, wenn eine Nachricht ins Log geschrieben wird.
        /// </summary>
        event EventHandler<LogEventArgs> MessageLogged;

        /// <summary>
        /// Liefert den Logger mit zu einem gegebenen Namen.
        /// </summary>
        /// <param name="name">Name des Loggers.</param>
        /// <returns>ILogger der mit dem Namen assoziiert ist.</returns>
        ILogger GetLogger( string name );

        /// <summary>
        /// Schreibt einen Eintrag direkt ins Log.
        /// </summary>
        /// <param name="logName">Name unter dem die Nachricht gelogt wird.</param>
        /// <param name="message">Inhalt der Nachricht.</param>
        /// <param name="time">Zeitpunkt an dem die Nachricht aufgetreten ist.</param>
        /// <param name="level">Bezeichnung des Levels der Nachricht (DEBUG,INFO,WARN,ERROR,FATAL)</param>
        void AddEntry(DateTime time, string level, string logName, string message, string exceptionText);

        /// <summary>
        /// Liefert im Log gespeicherte Einträge.
        /// </summary>
        /// <returns>Array von LogEventArgs die die geloggten Nachrichten repräsentieren.</returns>
        IList<LogEventArgs> GetEntries();
    }
}
