using System;

namespace Hamster.Plugin.Events
{
    public class SimpleEventTrigger : IEventTrigger
    {
        private ILogger logger = NullLogger.Instance;

        /// <summary>
        /// Zeigt und setzt den Logger für diese Instanz.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value ?? NullLogger.Instance; }
        }

        /// <summary>
        /// Führt eine Funktion im Kontext des Triggers aus.
        /// </summary>
        /// <param name="functor">IEventFunctor der ausgeführt werden soll.</param>
        protected virtual void InternalInvoke(IEventFunctor functor)
        {
            functor.Invoke();
        }

        /// <summary>
        /// Führt eine Funktion im Kontext des Triggers aus.
        /// </summary>
        /// <param name="functor">IEventFunctor der ausgeführt werden soll.</param>
        public virtual void Invoke(IEventFunctor functor)
        {
            if (functor == null)
            {
                throw new ArgumentNullException("functor");
            }

            try
            {
                InternalInvoke(functor);
            }
            catch (Exception x)
            {
                logger.Error(x, "Error while invoking functor {0}.", functor);
            }
        }

        /// <summary>
        /// Führt einen EventHandler im Kontext des Triggers aus.
        /// </summary>
        /// <typeparam name="TSender">Klasse des Senders des Events.</typeparam>
        /// <typeparam name="TArgs">Klasse der Informationen des Events.</typeparam>
        /// <param name="handler">EventHandler der ausgelöst werden soll.</param>
        /// <param name="sender">Sender des Events.</param>
        /// <param name="args">Informationen über das Event.</param>
        public virtual void Invoke<TSender, TArgs>(EventHandler<TSender, TArgs> handler, TSender sender, TArgs args)
            where TArgs : EventArgs
        {
            if (handler != null)
            {
                EventFunctor<TSender, TArgs> functor = new EventFunctor<TSender, TArgs>(handler, sender, args);

                Invoke(functor);
            }
        }
    }
}
