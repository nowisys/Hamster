using System;

namespace Hamster.Plugin.Events
{
    public class SimpleEventTrigger : IEventTrigger
    {
        /// <summary>
        /// Logger for errors in the invokation of events.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Führt einen EventHandler im Kontext des Triggers aus.
        /// </summary>
        /// <typeparam name="TSender">Klasse des Senders des Events.</typeparam>
        /// <typeparam name="TArgs">Klasse der Informationen des Events.</typeparam>
        /// <param name="handler">EventHandler der ausgelöst werden soll.</param>
        /// <param name="sender">Sender des Events.</param>
        /// <param name="args">Informationen über das Event.</param>
        public virtual void Invoke<TSender, TArgs>(EventHandler<TSender, TArgs> handler, TSender sender, TArgs args)
        {
            try {
                handler?.Invoke(sender, args);
            } catch (Exception x) {
                Logger?.Error(x, "Error while invoking event {0}({1}, {2})", handler, sender, args);
            }
        }
    }
}
