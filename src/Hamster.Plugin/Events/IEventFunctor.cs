
namespace Hamster.Plugin.Events
{
    /// <summary>
    /// Interface das zum Ausführen einer beliebigen Funktion verwendet wird.
    /// </summary>
    public interface IEventFunctor
    {
        /// <summary>
        /// Löst die darunterliegende Funktion aus.
        /// </summary>
        void Invoke();
    }
}
