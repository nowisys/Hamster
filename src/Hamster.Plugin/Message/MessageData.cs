
namespace Hamster.Plugin.Message
{
    /// <summary>
    /// Repräsentiert Daten zu einer hinterlegten Textnachricht.
    /// </summary>
    public class MessageData
    {
        private string messageId;
        private string text;

        /// <summary>
        /// Erstellt eine neue Instanz von MessageData.
        /// </summary>
        public MessageData()
        {

        }

        /// <summary>
        /// Zeigt und setzt den Bezeichner der Nachricht.
        /// </summary>
        public string MessageId
        {
            get { return messageId; }
            set { messageId = value; }
        }

        /// <summary>
        /// Zeigt und setzt den Text der Nachricht.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }
}
