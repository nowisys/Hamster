
namespace Hamster.Plugin.Sequence
{
    /// <summary>
    /// Interface für die Berechnung von Sequenzen
    /// </summary>
    public interface ISequenceRule
    {
        /// <summary>
        /// Berechnet den nächsten Wert ausgehend vom gegebenen Wert.
        /// </summary>
        /// <param name="currentValue">Aktueller Wert der Sequenz.</param>
        /// <returns>Nächster Wert der Sequenz-</returns>
        int GetNext(int currentValue);
    }
}
