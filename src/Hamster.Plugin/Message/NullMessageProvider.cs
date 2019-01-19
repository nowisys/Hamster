
namespace Hamster.Plugin.Message
{
    /// <summary>
    /// Einfache Implementierung von IMessageProvider,
    /// die immer null zurückgibt.
    /// </summary>
    public class NullMessageProvider : IMessageProvider
    {
        public static NullMessageProvider Instance = new NullMessageProvider();

        public string GetMessage(string messageId)
        {
            return null;
        }
    }
}
