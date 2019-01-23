using System;

namespace Hamster.Plugin.Events
{
    /// <summary>
    /// Interface zum Auslösen von Events im Kontext eines Triggers.
    /// </summary>
    /// <remarks>
    /// Die Verwendung des Interfaces ermöglicht ein generisches Auslösen von Events,
    /// ohne über die Details des Vorgangs zu entscheiden. Das heißt die Implementation
    /// des Interfaces IEventTrigger entscheidet darüber, ob das Event synchron oder
    /// asynchron ausgeführt wird, bzw. wie und womit es synchronisiert wird. Die Entscheidung
    /// muss dann nicht bereits beim Programmieren der Events getroffen werden, sondern kann
    /// dann zum Beispiel auch durch eine Konfiguration dynamisch gewählt werden, oder zu einem
    /// späteren Zeitpunkt auch leicht geändert werden.
    /// </remarks>
    public interface IEventTrigger
    {
        /// <summary>
        /// Führt einen EventHandler im Kontext des Triggers aus.
        /// </summary>
        /// <typeparam name="TSender">Klasse des Senders des Events.</typeparam>
        /// <typeparam name="TArgs">Klasse der Informationen des Events.</typeparam>
        /// <param name="handler">EventHandler der ausgelöst werden soll.</param>
        /// <param name="sender">Sender des Events.</param>
        /// <param name="args">Informationen über das Event.</param>
        void Invoke<TSender, TArgs>(EventHandler<TSender, TArgs> handler, TSender sender, TArgs args);
    }
}
