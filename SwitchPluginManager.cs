using System;
using System.Collections.Generic;
using Hamster.Plugin;
using Hamster.Plugin.Configuration;

namespace Hamster
{
    public class SwitchPluginManager : IPluginManager
    {
        private List<string> managerPrefixes;
        private List<IPluginManager> subManagers;
        private IPluginManager defaultManager;
        private string pluginPrefix;
        private ILogger logger = NullLogger.Instance;

        public SwitchPluginManager(IPluginManager[] subManagers)
        {
            managerPrefixes = new List<string>();
            this.subManagers = new List<IPluginManager>();

            foreach (IPluginManager manager in subManagers)
            {
                AddSubManager(manager);
            }
        }

        public string PluginPrefix
        {
            get { return pluginPrefix; }
            set { pluginPrefix = value; }
        }

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value ?? NullLogger.Instance; }
        }

        public void AddSubManager(IPluginManager manager)
        {
            lock (managerPrefixes)
            {
                if (string.IsNullOrEmpty(manager.PluginPrefix))
                {
                    defaultManager = manager;
                }
                else
                {
                    string prefix = pluginPrefix == null ? manager.PluginPrefix : pluginPrefix + manager.PluginPrefix;

                    int index = managerPrefixes.BinarySearch(prefix);

                    if (index > 0)
                    {
                        subManagers[index] = manager;
                    }
                    else if (index <= -1)
                    {
                        index = (~index);
                        managerPrefixes.Insert(index, prefix);
                        subManagers.Insert(index, manager);
                    }
                }
            }
        }

        private IPluginManager GetManager(string pluginName)
        {
            lock (managerPrefixes)
            {
                int index = managerPrefixes.BinarySearch(pluginName.ToLowerInvariant());
                if (index == -1)
                {
                    // has no preceding entry so there is no matching manager
                    return defaultManager;
                }
                else if (index < -1)
                {
                    index = (~index) - 1;
                    if (pluginName.StartsWith(managerPrefixes[index], StringComparison.InvariantCultureIgnoreCase))
                    {
                        // found a matching manager
                        return subManagers[index];
                    }
                    else
                    {
                        // the located manager does not match
                        return defaultManager;
                    }
                }
                else
                {
                    // exact match found, must be the right manager
                    return subManagers[index];
                }
            }
        }

        private string GetPluginName(PluginConfiguration config)
        {
            return config.PluginName;
        }

        public void AddPlugin(PluginConfig config)
        {
            GetManager(config.Name).AddPlugin(config);
        }

        public void UpdatePlugin(PluginConfig config)
        {
            GetManager(config.Name).UpdatePlugin(config);
        }

        public void RemovePlugin(string name)
        {
            GetManager(name).RemovePlugin(name);
        }

        public IList<PluginConfig> GetPlugins()
        {
            List<PluginConfig> plugins = new List<PluginConfig>();

            if (defaultManager != null)
            {
                try
                {
                    plugins.AddRange(defaultManager.GetPlugins());
                }
                catch (Exception x)
                {
                    Logger.Error(x, "Error retrieving plugins from default plugin manager.");
                }
            }

            foreach (IPluginManager manager in subManagers)
            {
                if (manager != defaultManager)
                {
                    try
                    {
                        plugins.AddRange(manager.GetPlugins());
                    }
                    catch (Exception x)
                    {
                        Logger.Error(x, "Error while retrieving plugins from plugin manager with prefix '{0}'", manager.PluginPrefix);
                    }
                }
            }
            return plugins;
        }

        public PluginConfig GetPlugin(string name)
        {
            return GetManager(name).GetPlugin(name);
        }

        public IList<string> GetPluginNames()
        {
            List<string> plugins = new List<string>();

            if (defaultManager != null)
            {
                try
                {
                    plugins.AddRange(defaultManager.GetPluginNames());
                }
                catch (Exception x)
                {
                    Logger.Error(x, "Error retrieving plugins from default plugin manager.");
                }
            }

            foreach (IPluginManager manager in subManagers)
            {
                if (manager != defaultManager)
                {
                    try
                    {
                        plugins.AddRange(manager.GetPluginNames());
                    }
                    catch (Exception x)
                    {
                        Logger.Error(x, "Error while retrieving plugins from plugin manager with prefix '{0}'", manager.PluginPrefix);
                    }
                }
            }
            return plugins;
        }
    }
}
