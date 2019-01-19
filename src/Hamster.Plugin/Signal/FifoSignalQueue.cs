using System.Collections.Generic;

namespace Hamster.Plugin.Signal
{
    public class FifoSignalQueue : AbstractSignalQueue
    {
        private Queue<SignalQueueEventArgs> queue = new Queue<SignalQueueEventArgs>();

        public FifoSignalQueue()
        {

        }

        public override void Enqueue( SignalQueueEventArgs args )
        {
            queue.Enqueue( args );
        }

        protected override SignalQueueEventArgs Dequeue()
        {
            if( queue.Count > 0 )
            {
                return queue.Dequeue();
            }
            else
            {
                return null;
            }
        }
    }
}
