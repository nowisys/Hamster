using System;

namespace Hamster.Plugin
{
    public interface IStateProvider
    {
        event EventHandler StateChanged;

        IPluginState GetState();
    }
}
