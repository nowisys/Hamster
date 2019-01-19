using System.Collections.Generic;
using Hamster.Plugin.Events;

namespace Hamster.Plugin
{
    public interface IPluginDirectory
    {
        void Register( string name, IPlugin plugin );
        bool Unregister( string name );
        bool Contains( string name );

        void Clear();

        IPlugin GetPlugin( string name );
        IList<IPlugin> GetPlugins();

        event EventHandler<IPluginDirectory, PluginEventArgs> PluginRegistered;
        event EventHandler<IPluginDirectory, PluginEventArgs> PluginUnregistered;
    }
}
