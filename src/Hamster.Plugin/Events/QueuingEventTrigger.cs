using System;
using System.Threading;
using System.Collections.Concurrent;

namespace Hamster.Plugin.Events
{
    public class QueuingEventTrigger : SimpleEventTrigger
    {
        private AutoResetEvent enqueue = new AutoResetEvent(false);
        private ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

        public override void Invoke<TSender, TArgs>(EventHandler<TSender, TArgs> handler, TSender sender, TArgs args)
        {
            try {
                queue.Enqueue(() => base.Invoke(handler, sender, args));
                enqueue.Set();
            } catch (Exception x) {
                Logger?.Error(x, "Error while queuing event {0}({1}, {2})", handler, sender, args);
            }
        }

        public void RunForever()
        {
            while (true) {
                Action ev;
                if (queue.TryDequeue(out ev)) {
                    ev();
                } else {
                    enqueue.WaitOne();
                }
            }
        }
    }
}

