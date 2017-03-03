using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hamster.Plugin.Message;
using Hamster.Plugin;
using Hamster.Plugin.NHibernate;

namespace Hamster
{
    public class MetaMessageProvider : IMessageProvider
    {
        private ILogger logger = NullLogger.Instance;
        private List<IMessageProvider> providers = new List<IMessageProvider>();
        private Dictionary<IPlugin, IMessageProvider> pluginProviders = new Dictionary<IPlugin, IMessageProvider>();

        public MetaMessageProvider(IPluginDirectory plugins)
            : this(plugins, null)
        { }

        public MetaMessageProvider(IPluginDirectory plugins, IMessageProvider[] providers)
        {
            if (providers != null)
                this.providers.AddRange(providers);

            lock (this.providers)
            {
                plugins.PluginRegistered += PluginAdded;
                plugins.PluginUnregistered += PluginRemoved;

                foreach (IPlugin plugin in plugins.GetPlugins())
                {
                    AddProvider(plugin as NHibernatePlugin);
                }
            }
        }

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value ?? NullLogger.Instance; }
        }

        private void AddProvider(NHibernatePlugin database)
        {
            if (database != null && database.HasMapping(typeof(MessageData)))
            {
                NHibernateMessageProvider m = new NHibernateMessageProvider(database);
                if (!pluginProviders.ContainsKey(database))
                {
                    pluginProviders[database] = m;
                }
            }
        }

        private void PluginRemoved(IPluginDirectory sender, PluginEventArgs args)
        {
            lock (providers)
            {
                pluginProviders.Remove(args.Plugin);
            }
        }

        private void PluginAdded(IPluginDirectory sender, PluginEventArgs args)
        {
            NHibernatePlugin database = args.Plugin as NHibernatePlugin;
            if (database != null)
            {
                lock (providers)
                {
                    AddProvider(database);
                }
            }
        }

        public string GetMessage(string messageId)
        {
            lock (providers)
            {
                foreach (var p in providers)
                {
                    try
                    {
                        string result = p.GetMessage(messageId);
                        if (result != null)
                            return result;
                    }
                    catch (Exception x)
                    {
                        Logger.Error(x, "Error getting message '{0}'.", messageId);
                    }
                }

                foreach (var pair in pluginProviders)
                {
                    try
                    {
                        if (pair.Key.IsOpen)
                        {
                            string result = pair.Value.GetMessage(messageId);
                            if (result != null)
                                return result;
                        }
                    }
                    catch (Exception x)
                    {
                        Logger.Error(x, "Error getting message '{0}'.", messageId);
                    }
                }

                return null;
            }
        }
    }
}
