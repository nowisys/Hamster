using System;
using Hamster.Plugin.Signal;

namespace Hamster.Plugin.Events
{
    /// <summary>
    /// Implementiert IEventTrigger mittels einer SignalQueue.
    /// </summary>
    /// <remarks>
    /// Die Events werden über die SignalQueue ausgelöst. Dadurch entscheidet die Implementation
    /// der Queue über das Verhalten der Events. Diese Art von EventTrigger kann verwendet werden,
    /// um die Events nacheinander auszuführen.
    /// </remarks>
    public class SignalEventTrigger : SimpleEventTrigger
    {
        private class SignalHandler : ISignalHandler
        {
            public bool Handle(string handlerId, SignalPriority priority, object signal, bool persisted)
            {
                if (signal is IEventFunctor)
                {
                    ((IEventFunctor)signal).Invoke();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private string name;
        private ISignalQueue queue;
        private SignalHandler handler;

        /// <summary>
        /// Erstellt eine neue Instanz von SignalEventTrigger.
        /// </summary>
        /// <param name="name">Name den der Trigger gegenüber der ISignalQueue verwendet.</param>
        /// <param name="queue">ISignalQueue über die die Events ausgelöst werden.</param>
        public SignalEventTrigger(string name, ISignalQueue queue)
        {
            if (queue == null)
            {
                throw new ArgumentNullException("queue");
            }

            this.name = name;
            this.queue = queue;
            this.handler = new SignalHandler();

            queue.SetHandler(name, handler);
        }

        /// <summary>
        /// Führt eine Funktion im Kontext des Triggers aus.
        /// </summary>
        /// <param name="functor">IEventFunctor der ausgeführt werden soll.</param>
        protected override void InternalInvoke(IEventFunctor functor)
        {
            queue.Enqueue(name, SignalPriority.Normal, functor, false);
        }
    }
}
