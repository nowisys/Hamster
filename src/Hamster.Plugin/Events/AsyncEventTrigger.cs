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
        public override void Invoke<TSender, TArgs>(EventHandler<TSender, TArgs> handler, TSender sender, TArgs args)
        {
            try {
                ThreadPool.QueueUserWorkItem((s) => base.Invoke(handler, sender, args));
            } catch (Exception x) {
                Logger?.Error(x, "Error while queuing event {0}({1}, {2})", handler, sender, args);
            }
        }
    }
}
