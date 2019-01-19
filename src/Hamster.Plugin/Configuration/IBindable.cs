
namespace Hamster.Plugin.Configuration
{
    /// <summary>
    /// Interface zum Herstellen von Verbindungen zwischen Plugins.
    /// </summary>
    public interface IBindable
    {
        /// <summary>
        /// Verbindet ein Objekt unter einem Namen zur aktuellen Instanz.
        /// </summary>
        /// <param name="name">Name unter dem die Instanz bekannt gemacht wird.</param>
        /// <param name="instance">Objekt dass verbunden wird.</param>
        void Bind( string name, object instance );

        /// <summary>
        /// Wird aufgerufen um anzuzeigen, dass alle vorliegenden Bindings angewendet wurden.
        /// </summary>
        /// <exception cref="Hamster.Plugin.Configuration.BindingException">
        /// Wird geworfen wenn nicht alle benötigten Bindings geliefert wurden.
        /// </exception>
        void BindingComplete();
    }
}
