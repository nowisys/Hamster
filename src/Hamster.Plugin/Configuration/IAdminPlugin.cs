namespace Hamster.Plugin.Configuration
{
    /// <summary>
    /// Interface um die Admin-Oberfläche zu erweitern.
    /// </summary>
    public interface IAdminPlugin
    {
        /// <summary>
        /// Teilt dem Plugin mit, welcher Host für Admin-Erweiterungen verwendet werden soll.
        /// </summary>
        /// <param name="host">Der Host bei dem das Plugin Dienste registrieren kann.</param>
        void AddToHost(IAdminHost host);
    }
}
