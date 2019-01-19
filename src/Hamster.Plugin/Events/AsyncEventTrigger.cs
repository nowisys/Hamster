using System;
using System.Threading;

namespace Hamster.Plugin.Events
{
    /// <summary>
    /// Implementiert IEventTrigger mittels ThreadPool.
    /// </summary>
    /// <remarks>
    /// Die Events werden asynchron über den .NET ThreadPool ausgeführt. Dadurch wird verhindert,
    /// dass zu viele Events gleichzeitig ausgelöst werden können, und so den Prozessor blockieren.
    /// Außerdem werden die Threads im ThreadPool wieder verwendet, wodurch eine Anfrage für einen
    /// neuen Thread effizienter ist, als wenn er immer neu erzeugt werden müsste.
    /// </remarks>
    public class AsyncEventTrigger : SimpleEventTrigger
    {
        /// <summary>
        /// Erstellt eine neue Instanz von AsyncEventTrigger.
        /// </summary>
        public AsyncEventTrigger()
        {
        }

        /// <summary>
        /// Führt eine Funktion im Kontext des Triggers aus.
        /// </summary>
        /// <param name="functor">IEventFunctor der ausgeführt werden soll.</param>
        protected override void InternalInvoke(IEventFunctor functor)
        {
            ThreadPool.QueueUserWorkItem(InvokeCallback, functor);
        }

        /// <summary>
        /// Callback der innerhalb des Threadpools ausfegührt wird.
        /// </summary>
        /// <param name="state">Enthält den IEventFunctor der ausgeführt werden soll.</param>
        protected virtual void InvokeCallback(object state)
        {
            IEventFunctor functor = (IEventFunctor)state;

            try
            {
                functor.Invoke();
            }
            catch (Exception x)
            {
                Logger.Error("Error while invoking event handler.", x);
            }
        }
    }
}
