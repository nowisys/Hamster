
namespace Hamster.Plugin.Sequence
{
    /// <summary>
    /// Interface zum verwenden von Sequenzen.
    /// </summary>
    public interface ISequenceProvider
    {
        /// <summary>
        /// Liefert den Wert der Sequenz und setzt diese einen Schritt weiter.
        /// Falls die Sequenz nicht existiert, wird sie angelegt mit dem gegebenen Startwert.
        /// </summary>
        /// <param name="sequenceId">Bezeichnung der Sequenz.</param>
        /// <param name="startValue">Erster wert für die Sequenz.</param>
        /// <returns>Den Wert der Sequenz oder null falls die Sequenz nicht existiert.</returns>
        int GetNext(string sequenceId, int startValue);

        /// <summary>
        /// Liefert den Wert der Sequenz und setzt diese einen Schritt weiter.
        /// </summary>
        /// <param name="sequenceId">Bezeichnung der Sequenz.</param>
        /// <returns>Den Wert der Sequenz oder null falls die Sequenz nicht existiert.</returns>
        int? GetNext( string sequenceId );

        /// <summary>
        /// Erstellt eine neue Sequenz oder verändert eine bestehende Sequenz.
        /// </summary>
        /// <param name="sequenceId">Bezeichnung der Sequenz.</param>
        /// <param name="value">Startwert der Sequenz.</param>
        /// <param name="rule">Name der Regel mit der die Sequenz berechnet wird. Mit null wird die Standardregel verwendet.</param>
        void SetSequence( string sequenceId, int value, string rule );

        /// <summary>
        /// Löscht eine Sequenz.
        /// </summary>
        /// <param name="sequenceId">Bezeichnung der Sequenz.</param>
        /// <returns>True falls die Sequenz gelöscht wurde; false sonst.</returns>
        bool RemoveSequence( string sequenceId );

        /// <summary>
        /// Holt alle Daten zu einer Sequenz.
        /// </summary>
        /// <param name="sequenceId">Bezeichnung der Sequenz.</param>
        /// <returns>Daten zu der angegeben Sequenz oder null falls sie nicht vorhanden ist.</returns>
        SequenceItem GetSequence( string sequenceId );

        /// <summary>
        /// Hold Daten zu allen Sequenzen.
        /// </summary>
        /// <returns>Arraz von Daten zu allen Sequenzen.</returns>
        SequenceItem[] GetSequences();
    }
}
