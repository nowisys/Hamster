
using System.Collections.Generic;
namespace Hamster.Plugin.Message
{
    /// <summary>
    /// Interface zum zugriff auf Textnachrichten.
    /// </summary>
    public interface IMessageProvider
    {
        /// <summary>
        /// Liefert den Text zu einer bestimmten Nachricht.
        /// </summary>
        /// <param name="messageId">Bezeichner der Nachricht.</param>
        /// <returns>Text zu der Nachricht.</returns>
        string GetMessage( string messageId );
    }
}
