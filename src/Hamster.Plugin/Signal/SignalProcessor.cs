using System;
using System.Threading;

namespace Hamster.Plugin.Signal
{
    public class SignalProcessor
    {
        private string threadName;
        private Thread thread;
        private AbstractSignalQueue queue;
        private bool active = false;
        private int timeout = 1000;
        private ILogger logger = NullLogger.Instance;

        public SignalProcessor( AbstractSignalQueue queue )
        {
            if( queue == null )
                throw new ArgumentNullException( "queue" );

            this.queue = queue;
            threadName = GetType().Name;
        }

        public string ThreadName
        {
            get { return threadName; }
            set { threadName = value; }
        }

        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value ?? NullLogger.Instance; }
        }

        public virtual void Start()
        {
            if( !active )
            {
                active = true;
                thread = new Thread( ProcessLoop );
                thread.Name = threadName;
                thread.Start();
            }
        }

        protected virtual void ProcessLoop()
        {
            while( active )
            {
                try
                {
                    if( queue.WaitForMessage( timeout ) )
                    {
                        queue.DistributeMessage();
                    }
                }
                catch( Exception x )
                {
                    logger.Error( "An error occured while processing a queued signal.", x );
                }
            }
        }

        public virtual void Stop()
        {
            if( active )
            {
                active = false;
                thread.Join();
            }
        }
    }
}
