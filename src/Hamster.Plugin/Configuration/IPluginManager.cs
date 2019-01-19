using System.Collections.Generic;

namespace Hamster.Plugin.Configuration
{
    public interface IPluginManager
    {
        string PluginPrefix { get; }

        IList<string> GetPluginNames();
        IList<PluginConfig> GetPlugins();

        PluginConfig GetPlugin(string name);
        void AddPlugin(PluginConfig config);
        void UpdatePlugin(PluginConfig config);
        void RemovePlugin(string name);
    }
}
