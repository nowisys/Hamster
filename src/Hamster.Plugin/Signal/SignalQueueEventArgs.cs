using System;

namespace Hamster.Plugin.Signal
{
    [Serializable()]
    public class SignalQueueEventArgs : EventArgs
    {
        private string handler;
        private SignalPriority priority;
        private object signal;
        private bool persisted;
        private bool dequeued;
        private object syncRoot = new object();

        public SignalQueueEventArgs( string handler, SignalPriority priority, object signal, bool persisted )
        {
            this.handler = handler;
            this.priority = priority;
            this.signal = signal;
            this.persisted = persisted;
        }

        public string Handler
        {
            get { return handler; }
        }

        public SignalPriority Priority
        {
            get { return priority; }
        }

        public object Signal
        {
            get { return signal; }
        }

        public bool Persisted
        {
            get { return persisted; }
        }

        public bool Dequeue()
        {
            lock( syncRoot )
            {
                if( dequeued )
                {
                    return false;
                }
                dequeued = true;
                return true;
            }
        }
    }
}
