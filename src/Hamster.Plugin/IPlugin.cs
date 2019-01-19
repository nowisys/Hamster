using Hamster.Plugin.IoC;
using System;

namespace Hamster.Plugin
{
    /// <summary>
    /// Interface für ein hServer Plugin.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Zeigt und setzt den Name des Plugins.
        /// </summary>
        /// <remarks>
        /// Sollte nur vom Kernel vor der Initialisierung gesetzt werden.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">Wird bei einem 'set' geworfen, wenn die Initialisierung bereits abgeschlossen ist.</exception>
        string Name { get; set; }
        /// <summary>
        /// Zeigt und setzt den Logger, den das Plugin verwendet.
        /// </summary>
        /// <remarks>
        /// Sollte nur vom Kernel vor der Initialisierung gesetzt werden.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">Wird bei einem 'set' geworfen, wenn die Initialisierung bereits abgeschlossen ist.</exception>
        ILogger Logger { get; set; }

        /// <summary>
        /// Zeigt und setzt den ServiceProvider, um implementierende Services für Schnittstellen zu definieren und abzufragen.
        /// </summary>
        IPluginServiceProvider PluginServiceProvider { get; set; }

        /// <summary>
        /// Zeigt an, ob das Plugin geöffnet ist.
        /// </summary>
        bool IsOpen { get; }
        /// <summary>
        /// Zeigt an, ob das Plugin einsatzbereit ist.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Wird aufgerufen, wenn das Plugin gestartet wird.
        /// </summary>
        event EventHandler Opened;
        /// <summary>
        /// Wird aufgerufen, wenn das Plugin gestoppt wird.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Wird aufgerufen, um die Initialisierung und Konfiguration abzuschließen.
        /// </summary>
        /// <remarks>
        /// Sollte nur vom Kernel aufgerufen werden.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">Wird geworfen, wenn die Initialisierung bereits durchgeführt wurde.</exception>
        void Init();

        /// <summary>
        /// Startet das Plugin.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Wird geworfen, wenn das Plugin nicht initialisiert wurde.</exception>
        void Open();
        /// <summary>
        /// Stoppt das Plugin.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Wird geworfen, wenn das Plugin nicht initialisiert wurde.</exception>
        void Close();
    }
}
