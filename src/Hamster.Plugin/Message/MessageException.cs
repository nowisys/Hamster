using System;
using System.Collections.Generic;

namespace Hamster.Plugin.Message
{
    [global::System.Serializable]
    public class MessageException : Exception
    {
        public MessageException(string messageId)
            : this(messageId, "Error " + messageId, null)
        {
        }

        public MessageException(string messageId, string message)
            : this(messageId, message, null)
        {
        }

        public MessageException(string messageId, string message, Exception inner)
            : base(message, inner)
        {
            Parameters = new Dictionary<string, object>();
            MessageId = messageId;
        }

        public string MessageId { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; }
    }
}
