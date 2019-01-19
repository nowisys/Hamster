using System;

namespace Hamster.Plugin.Events
{
    /// <summary>
    /// Implementiert IEventFunctor mit einem EventHandler-Delegate.
    /// </summary>
    /// <typeparam name="TSender">Klasse des Auslösers der Events.</typeparam>
    /// <typeparam name="TArgs">Klasse der Daten des Events.</typeparam>
    public class EventFunctor<TSender, TArgs> : IEventFunctor
            where TArgs : EventArgs
    {
        private TSender sender;
        private TArgs args;
        private EventHandler<TSender, TArgs> handler;

        /// <summary>
        /// Erzeugt eine neue Instanz von EventFunctor.
        /// </summary>
        /// <param name="handler">EventHandler die ausgelöst werden sollen.</param>
        /// <param name="sender">Wert für den Parameter "sender" der an die EventHandler übergeben wird.</param>
        /// <param name="args">Wert für den Parameter "args" der an die EventHandler übergeben wird.</param>
        public EventFunctor(EventHandler<TSender, TArgs> handler, TSender sender, TArgs args)
        {
            this.sender = sender;
            this.args = args;
            this.handler = handler;
        }

        /// <summary>
        /// Löst das Event mit den gespeicherten Parametern aus.
        /// </summary>
        public void Invoke()
        {
            if (handler != null)
            {
                handler(sender, args);
            }
        }
    };
}
