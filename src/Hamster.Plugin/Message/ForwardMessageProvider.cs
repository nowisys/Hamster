
using System;
using System.Collections.Generic;
namespace Hamster.Plugin.Message
{
    /// <summary>
    /// Implementiert IMessageProvider und reicht die Befehle an einen übergeordneten Provider weiter.
    /// </summary>
    /// <remarks>
    /// Der ForwardMessageProvider erweitert den angefragten Bezeichner einer Sequenz um eine
    /// definierte Zeichenkette und reicht die Befehle mit dem neuen Bezeichner weiter.
    /// Der neue Bezeichner setzt sich wie folgt zusammen: {Erweiterung}.{alter Bezeichner}
    /// </remarks>
    public class ForwardMessageProvider : IMessageProvider
    {
        private string name;
        private IMessageProvider parent;

        /// <summary>
        /// Erstellt eine neue Instanz von ForwardMessageProvider.
        /// </summary>
        /// <param name="name">String mit dem die Bezeichner erweitert werden.</param>
        /// <param name="parent">ISequenceProvider an den die Befehle weitergereicht werden.</param>
        /// <exception cref="System.ArgumentNullException">Wird geworfen wenn der Parameter 'parent' null ist.</exception>
        public ForwardMessageProvider( string name, IMessageProvider parent )
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
        public IMessageProvider Parent
        {
            get { return parent; }
            set
            {
                if( parent == null )
                {
                    throw new ArgumentNullException();
                }
                parent = value;
            }
        }

        /// <summary>
        /// Erzeugt einen Untergeordneten Provider mit der zusätlichen Erweiterung.
        /// </summary>
        /// <remarks>
        /// Der neue IMessageProvider reicht seine Befehle an den gleichen Übergeordneten Provider weiter.
        /// Dadurch bleibt die Hierarchie flach und die ForwardMessageProvider bleiben unabhängig voneinander.
        /// Wenn das nicht gewünscht ist, kann der normale Konstruktor von ForwardMessageProvider verwendet werden.
        /// </remarks>
        /// <param name="name">String mit dem die Bezeichner des neuen Providers erweitert werden, zusätzlich zur Erweiterung der aktuellen Instanz.</param>
        /// <returns>Untergeordneter IMessageProvider mit neuer Erweiterung.</returns>
        public virtual IMessageProvider CreateSubProvider( string name )
        {
            return new ForwardMessageProvider( this.name + "." + name, parent );
        }

        /// <summary>
        /// Liefert den Text zu einer bestimmten Nachricht.
        /// </summary>
        /// <param name="messageId">Bezeichner der Nachricht.</param>
        /// <returns>Text zu der Nachricht.</returns>
        public virtual string GetMessage( string messageId )
        {
            return parent.GetMessage( name + "." + messageId );
        }
    }
}
