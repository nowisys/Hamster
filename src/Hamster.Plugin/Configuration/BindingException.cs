using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hamster.Plugin.Configuration
{
    /// <summary>
    /// Zeigt an, dass es einen Fehler beim Binding-Vorgang gab.
    /// </summary>
    [Serializable()]
    public class BindingException : Exception
    {
        private const string DefaultText = "Not all required bindings have been set. The following bindings are missing: {0}";
        private List<string> requiredBindings;

        /// <summary>
        /// Erstellt eine neue Instanz von BindingException.
        /// </summary>
        /// <param name="requiredBindings">Namen der noch benötigten Bindings.</param>
        public BindingException( string[] requiredBindings )
            : this( string.Format( DefaultText, string.Join( ", ", requiredBindings ) ), null, requiredBindings )
        {
        }

        /// <summary>
        /// Erstellt eine neue Instanz von BindingException.
        /// </summary>
        /// <param name="message">Beschreibung der Exception.</param>
        /// <param name="requiredBindings">Namen der noch benötigten Bindings.</param>
        public BindingException( string message, params string[] requiredBindings )
            : this( message, null, requiredBindings )
        {
        }

        /// <summary>
        /// Erstellt eine neue Instanz von BindingException.
        /// </summary>
        /// <param name="message">Beschreibung der Exception.</param>
        /// <param name="requiredBindings">Namen der noch benötigten Bindings.</param>
        public BindingException( string message, Exception inner, params string[] requiredBindings )
            : base( message ?? DefaultText, inner )
        {
            this.requiredBindings = new List<string>(requiredBindings);
        }

        /// <summary>
        /// Liefert eine Liste (read-only) von
        /// </summary>
        public IList<string> RequiredBindings
        {
            get { return requiredBindings; }
        }
    }
}
