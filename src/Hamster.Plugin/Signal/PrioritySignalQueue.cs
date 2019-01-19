using System.Collections.Generic;

namespace Hamster.Plugin.Signal
{
    public class PrioritySignalQueue : AbstractSignalQueue
    {
        private Queue<SignalQueueEventArgs>[] queues;

        public PrioritySignalQueue()
        {
            queues = new Queue<SignalQueueEventArgs>[]
            {
                new Queue<SignalQueueEventArgs>(),
                new Queue<SignalQueueEventArgs>(),
                new Queue<SignalQueueEventArgs>(),
                new Queue<SignalQueueEventArgs>(),
                new Queue<SignalQueueEventArgs>(),
            };
        }

        public override void Enqueue( SignalQueueEventArgs args )
        {
            queues[(int)args.Priority].Enqueue( args );
        }

        protected override SignalQueueEventArgs Dequeue()
        {
            for( int i = 0; i < queues.Length; ++i )
            {
                if( queues[i].Count > 0 )
                {
                    return queues[i].Dequeue();
                }
            }
            return null;
        }
    }
}
