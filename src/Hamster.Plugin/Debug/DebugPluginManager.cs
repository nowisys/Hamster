using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hamster.Plugin;
using Hamster.Plugin.Configuration;

namespace Hamster.Plugin.Debug
{
    public class DebugPluginManager : IPluginManager
    {
        private ILogger logger = NullLogger.Instance;
        private List<PluginConfig> plugins = new List<PluginConfig>();

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value ?? NullLogger.Instance; }
        }

        public List<PluginConfig> Plugins
        {
            get { return plugins; }
        }

        public void AddPlugin(PluginConfig config)
        {
            string settings = config.Settings != null ? config.Settings.OuterXml : "(null)";
            Logger.Info("AddPlugin('{0}', '{1}', '{2}')", config.Name, config.Type, settings);

            if (plugins.Find(x => x.Name == config.Name) != null)
                throw new ArgumentException(string.Format("There is already a plugin with the name '{0}'.", config.Name));
            plugins.Add(config);
        }

        public PluginConfig GetPlugin(string name)
        {
            var result = from p in GetPlugins() where p.Name == name select p;
            return result.FirstOrDefault();
        }

        public IList<string> GetPluginNames()
        {
            var names = from p in GetPlugins() select p.Name;
            return names.ToArray();
        }

        public IList<PluginConfig> GetPlugins()
        {
            return plugins.AsReadOnly();
        }

        public string PluginPrefix
        {
            get { return string.Empty; }
        }

        public void RemovePlugin(string name)
        {
            Logger.Info("RemovePlugin( {0} )", name);
            plugins.RemoveAll(x => x.Name == name);
        }

        public void UpdatePlugin(PluginConfig config)
        {
            string settings = config.Settings != null ? config.Settings.OuterXml : "(null)";
            Logger.Info("UpdatePlugin('{0}', '{1}', '{2}')", config.Name, config.Type, settings);
            RemovePlugin(config.Name);
            plugins.Add(config);
        }
    }
}
