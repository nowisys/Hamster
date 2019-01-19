using System;

namespace Hamster.Plugin.Sequence
{
    /// <summary>
    /// Repräsentiert alle Daten zu einer Sequenz.
    /// </summary>
    [Serializable()]
    public class SequenceItem
    {
        private string sequenceId;
        private int value;
        private string rule;
        private DateTime lastChange;

        /// <summary>
        /// Erstellt eine neue Instanz von SequenceItem
        /// </summary>
        public SequenceItem()
        {

        }

        /// <summary>
        /// Zeigt und setzt den Bezeichner der Sequenz.
        /// </summary>
        public string SequenceId
        {
            get { return sequenceId; }
            set { sequenceId = value; }
        }

        /// <summary>
        /// Zeigt und setzt den aktuellen Wert der Sequenz.
        /// </summary>
        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Zeigt und setzt den Namen der Regel mit der die Sequenz berechnet wird.
        /// </summary>
        public string Rule
        {
            get { return rule; }
            set { rule = value; }
        }

        /// <summary>
        /// Zeigt und setzt den Zeitpunkt der letzten Änderung an der Sequenz.
        /// </summary>
        public DateTime LastChange
        {
            get { return lastChange; }
            set { lastChange = value; }
        }
    }
}
