using System.Collections.Generic;
using System.Threading;

namespace Hamster.Plugin.Signal
{
    public abstract class AbstractSignalQueue : ISignalQueue
    {
        private ILogger logger = NullLogger.Instance;
        private ManualResetEvent queueEvent;
        private Dictionary<string, ISignalHandler> handlerIndex = new Dictionary<string, ISignalHandler>();

        public AbstractSignalQueue()
        {
            queueEvent = new ManualResetEvent( false );
        }

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value ?? NullLogger.Instance; }
        }

        public virtual void SetHandler( string id, ISignalHandler handler )
        {
            handlerIndex[id] = handler;
        }

        public virtual void Enqueue( string handler, SignalPriority priority, object signal, bool persist )
        {
            Enqueue( new SignalQueueEventArgs( handler, priority, signal, persist ) );
        }

        public virtual bool WaitForMessage( int timeout )
        {
            return queueEvent.WaitOne( timeout );
        }

        public virtual void DistributeMessage()
        {
            SignalQueueEventArgs args = Dequeue();
            if( args != null && args.Dequeue() )
            {
                ISignalHandler handler;
                if( handlerIndex.TryGetValue( args.Handler, out handler ) )
                {
                    handler.Handle( args.Handler, args.Priority, args.Signal, args.Persisted );
                }
                else
                {
                    logger.Warn( "Could not find handler '{0}' for message.", args.Handler );
                }
            }
        }

        public abstract void Enqueue( SignalQueueEventArgs args );
        protected abstract SignalQueueEventArgs Dequeue();
    }
}
