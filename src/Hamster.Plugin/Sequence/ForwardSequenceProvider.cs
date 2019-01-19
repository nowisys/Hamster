using System;

namespace Hamster.Plugin.Sequence
{
    /// <summary>
    /// Implementiert ISequenceProvider und reicht die Befehle an einen übergeordneten Provider weiter.
    /// </summary>
    /// <remarks>
    /// Der ForwardSequenceProvider erweitert den angefragten Bezeichner einer Sequenz um eine
    /// definierte Zeichenkette und reicht die Befehle mit dem neuen Bezeichner weiter.
    /// Der neue Bezeichner setzt sich wie folgt zusammen: {Erweiterung}.{alter Bezeichner}
    /// </remarks>
    public class ForwardSequenceProvider : ISequenceProvider
    {
        private string name;
        private ISequenceProvider parent;

        /// <summary>
        /// Erstellt eine neue Instanz von ForwardSequenceProvider.
        /// </summary>
        /// <param name="name">String mit dem die Bezeichner erweitert werden.</param>
        /// <param name="parent">ISequenceProvider an den die Befehle weitergereicht werden.</param>
        public ForwardSequenceProvider( string name, ISequenceProvider parent )
        {
            if( parent == null )
            {
                throw new ArgumentNullException( "parent" );
            }

            this.name = name;
            this.parent = parent;
        }

        /// <summary>
        /// Zeigt und setzt die Zeichenkette mit der die Bezeichner erweitert werden.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Zeigt und setzt den ISequenceProvider, an den die Befehle weitergereicht werden.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Wird geworfen wenn auf null gesetzt werden soll.</exception>
        public ISequenceProvider Parent
        {
            get { return parent; }
            set
            {
                if( value == null )
                {
                    throw new ArgumentNullException( "value" );
                }

                parent = value;
            }
        }

        /// <summary>
        /// Erzeugt den neuen Bezeichner.
        /// </summary>
        /// <param name="sequenceId">Bezeichner zu dem ein neuer Bezeichner erstellt wird.</param>
        /// <returns>Erweiterter Bezeichner der Sequenz.</returns>
        protected virtual string BuildSequenceId( string sequenceId )
        {
            return name + "." + sequenceId;
        }

        /// <summary>
        /// Liefert den Wert der Sequenz und setzt diese einen Schritt weiter.
        /// Falls die Sequenz nicht existiert, wird sie angelegt mit dem gegebenen Startwert.
        /// </summary>
        /// <param name="sequenceId">Bezeichnung der Sequenz.</param>
        /// <param name="startValue">Erster wert für die Sequenz.</param>
        /// <returns>Den Wert der Sequenz oder null falls die Sequenz nicht existiert.</returns>
        public int GetNext(string sequenceId, int startValue)
        {
            return parent.GetNext(BuildSequenceId(sequenceId), startValue);
        }

        /// <summary>
        /// Liefert den Wert der Sequenz und setzt diese einen Schritt weiter.
        /// </summary>
        /// <param name="sequenceId">Bezeichnung der Sequenz.</param>
        /// <returns>Den Wert der Sequenz oder null falls die Sequenz nicht existiert.</returns>
        public int? GetNext( string sequenceId )
        {
            return parent.GetNext( BuildSequenceId( sequenceId ) );
        }

        /// <summary>
        /// Erstellt eine neue Sequenz oder verändert eine bestehende Sequenz.
        /// </summary>
        /// <param name="sequenceId">Bezeichnung der Sequenz.</param>
        /// <param name="value">Startwert der Sequenz.</param>
        /// <param name="rule">Name der Regel mit der die Sequenz berechnet wird. Mit null wird die Standardregel verwendet.</param>
        public void SetSequence( string sequenceId, int value, string rule )
        {
            parent.SetSequence( BuildSequenceId( sequenceId ), value, rule );
        }

        /// <summary>
        /// Löscht eine Sequenz.
        /// </summary>
        /// <param name="sequenceId">Bezeichnung der Sequenz.</param>
        /// <returns>True falls die Sequenz gelöscht wurde; false sonst.</returns>
        public bool RemoveSequence( string sequenceId )
        {
            return parent.RemoveSequence( BuildSequenceId( sequenceId ) );
        }

        /// <summary>
        /// Holt alle Daten zu einer Sequenz.
        /// </summary>
        /// <param name="sequenceId">Bezeichnung der Sequenz.</param>
        /// <returns>Daten zu der angegeben Sequenz oder null falls sie nicht vorhanden ist.</returns>
        public SequenceItem GetSequence( string sequenceId )
        {
            return parent.GetSequence( BuildSequenceId( sequenceId ) );
        }

        /// <summary>
        /// Hold Daten zu allen Sequenzen.
        /// </summary>
        /// <returns>Arraz von Daten zu allen Sequenzen.</returns>
        public SequenceItem[] GetSequences()
        {
            return parent.GetSequences();
        }
    }
}
