using System.Collections.Generic;
using System.Linq;
using Hamster.Plugin.Events;

namespace Hamster.Plugin.Debug
{
    public class DebugPluginDirectory : IPluginDirectory
    {
        private ILogger logger = NullLogger.Instance;

        private Dictionary<string, IPlugin> items = new Dictionary<string, IPlugin>();

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value ?? NullLogger.Instance; }
        }

        public event EventHandler<IPluginDirectory, PluginEventArgs> PluginRegistered;

        public event EventHandler<IPluginDirectory, PluginEventArgs> PluginUnregistered;

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(string name)
        {
            return items.ContainsKey(name);
        }

        public IPlugin GetPlugin(string name)
        {
            IPlugin result;
            if (!items.TryGetValue(name, out result))
            {
                result = null;
            }
            return result;
        }

        public IList<IPlugin> GetPlugins()
        {
            return items.Values.ToList();
        }

        public void Register(string name, IPlugin plugin)
        {
            items[name] = plugin;

            EventHandler<IPluginDirectory, PluginEventArgs> handler = PluginRegistered;
            if (handler != null)
            {
                handler(this, new PluginEventArgs(plugin));
            }
        }

        public bool Unregister(string name)
        {
            IPlugin plugin;
            if (items.TryGetValue(name, out plugin))
            {
                items.Remove(name);
                EventHandler<IPluginDirectory, PluginEventArgs> handler = PluginUnregistered;
                handler(this, new PluginEventArgs(plugin));
            }

            return plugin != null;
        }
    }
}
