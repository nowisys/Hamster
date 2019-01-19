using System;
using System.Collections.Generic;
using System.Text;

namespace Hamster.Plugin.Signal
{
    public delegate bool SignalHandlerCallback(string handlerId, SignalPriority priority, object signal, bool persisted);

    public class SignalHandlerFunctor : ISignalHandler
    {
        private SignalHandlerCallback callback;

        public SignalHandlerFunctor(SignalHandlerCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            this.callback = callback;
        }

        public bool Handle(string handlerId, SignalPriority priority, object signal, bool persisted)
        {
            return callback(handlerId, priority, signal, persisted);
        }
    }
}
