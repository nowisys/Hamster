
namespace Hamster.Plugin.Signal
{
    /// <summary>
    /// Interface zum Hinterlegen von Signalen.
    /// </summary>
    public interface ISignalQueue
    {
        /// <summary>
        /// Registriert einen Handler.
        /// </summary>
        /// <param name="id">Id unter der der ISignalHandler abgelegt wird.</param>
        /// <param name="handler">ISignalHandler der die Signale bearbeitet.</param>
        /// <exception cref="System.ArgumentNullException">Ein Parameter hat den Wert null.</exception>
        void SetHandler( string id, ISignalHandler handler );

        /// <summary>
        /// Legt ein Signal in der Queue ab.
        /// </summary>
        /// <param name="handler">Ziel des Signals.</param>
        /// <param name="priority">Priorität des Signals.</param>
        /// <param name="signal">Objekt dass als Signal behandelt wird.</param>
        /// <param name="persist">Zeigt an ob das Signal dauerhaft gespeichert werden soll.</param>
        /// <exception cref="System.ArgumentNullException">Ein Parameter hat den Wert null.</exception>
        void Enqueue( string handler, SignalPriority priority, object signal, bool persist );
    }
}
