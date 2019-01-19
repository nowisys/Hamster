
namespace Hamster.Plugin.Signal
{
    /// <summary>
    /// Interface dass die Behandlung von Signalen ermöglicht.
    /// </summary>
    public interface ISignalHandler
    {
        /// <summary>
        /// Lässt ein Signal Handler bearbeiten.
        /// </summary>
        /// <param name="handlerId">Der Name dem das Signal zugeordet wurde.</param>
        /// <param name="priority">Die Priorität des Signals.</param>
        /// <param name="signal">Das eigentliche Signal.</param>
        /// <param name="persisted">Zeigt an, ob das Signal dauerhaft ist.</param>
        /// <returns>True wenn das Signal erfolgreich behandelt wurde; false wenn es später nocheinmal aufgerufen werden soll.</returns>
        bool Handle( string handlerId, SignalPriority priority, object signal, bool persisted );
    }
}
