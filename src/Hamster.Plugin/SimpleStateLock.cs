using System;
using System.Threading;

namespace Hamster.Plugin
{
    class SimpleStateLock : IDisposable
    {
        private object syncRoot;

        public SimpleStateLock( object syncRoot )
        {
            this.syncRoot = syncRoot;
            Monitor.Enter( this.syncRoot );
        }

        public void Dispose()
        {
            Monitor.Exit( syncRoot );
        }
    }
}
