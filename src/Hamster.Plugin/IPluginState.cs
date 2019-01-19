
namespace Hamster.Plugin
{
    /// <summary>
    /// Interface für den Status der Plugins
    /// </summary>
    public interface IPluginState
    {
        /// <summary>
        /// Zeigt an, ob das Plugin aktiviert ist
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Zeigt an, ob das Plugin bereit zur Verarbeitung ist
        /// </summary>
        bool Ready { get; set; }

        IPluginState CreateDeepCopy();
    }
}
