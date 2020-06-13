using System;
using System.Collections.Generic;
using Hamster.Plugin.Configuration;
using System.Xml;

namespace Hamster.Plugin.Debug
{
    public class DebugKernel : IDisposable, IServiceProvider
    {
        private ILogger logger;
        private readonly IObjectFactory factory;
        private readonly IPluginManager manager;
        private readonly IPluginDirectory plugins;

        //private Dictionary<string, IPlugin> plugins;

        public DebugKernel()
            : this("settings")
        {
        }

        public DebugKernel(string directory)
            : this(new FilePluginManager(directory))
        {
        }

        public DebugKernel(IPluginManager manager)
        {
            this.manager = manager;
            logger = new DebugLogger("Debug");
            factory = new ObjectFactory(this);
            plugins = new DebugPluginDirectory();
        }

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value ?? NullLogger.Instance; }
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ILogger)) {
                return logger;
            } else if (serviceType == typeof(IObjectFactory)) {
                return factory;
            } else if (serviceType == typeof(IPluginManager)) {
                return manager;
            } else if (serviceType == typeof(IServiceProvider)) {
                return this;
            }

            return null;
        }

        public virtual void Configure(IPlugin plugin, XmlElement settings)
        {
            if (plugin is IXmlConfigurable config && !config.IsConfigured)
            {
                config.Configure(settings);
            }
        }

        public virtual void Bind(IPlugin plugin, IEnumerable<PluginBind> bindings)
        {
            if (plugin is IBindable bindablePlugin)
            {
                foreach (var bind in bindings)
                {
                    try
                    {
                        bindablePlugin.Bind(bind.Slot, GetPlugin(bind.Plugin));
                    }
                    catch (Exception x)
                    {
                        throw new BindingException(string.Format("Could not resolve binding to '{0}'.", bind.Plugin), x);
                    }
                }
                bindablePlugin.BindingComplete();
            }
        }

        /// <summary>
        /// Konfiguriert und initialisiert ein Plugin.
        /// </summary>
        /// <param name="plugin">Plugin das konfiguriert wird</param>
        /// <param name="config">Konfiguration die verwendet wird</param>
        public virtual void SetupPlugin(IPlugin plugin, PluginConfig config)
        {
            plugin.Name = config.Name;
            plugin.Logger = logger.CreateChildLogger(config.Name);

            Configure(plugin, config.Settings);
            Bind(plugin, config.Bindings);
            plugin.Init();

            plugins.Register(plugin.Name, plugin);
        }

        public virtual IPlugin GetPlugin(string name)
        {
            return plugins.GetPlugin(name);
        }

        /// <summary>
        /// Erstellt, konfiguriert und initialisiert ein Plugin aus einer Datei.
        /// </summary>
        /// <param name="name">Name des Plugins</param>
        /// <returns>Plugininstanz</returns>
        public virtual IPlugin LoadPlugin(string name)
        {
            var config = manager.GetPlugin(name);

            Type type = Type.GetType(config.Type);
            IPlugin result = (IPlugin)factory.Create(type, new Dictionary<string, object> {{"Plugins" , plugins}});
            SetupPlugin(result, config);
            return result;
        }

        /// <summary>
        /// Konfiguriert und initialisiert ein Plugin aus einer Datei.
        /// </summary>
        /// <param name="name">Name des Plugins</param>
        /// <param name="plugin">Instanz die konfiguriert wird</param>
        public virtual void LoadPlugin(string name, IPlugin plugin)
        {
            var config = manager.GetPlugin(name);
            SetupPlugin(plugin, config);
        }

        /// <summary>
        /// Öffnet das Plugin
        /// </summary>
        /// <param name="name">Name des Plugins<</param>
        public virtual void OpenPlugin(string name)
        {
            var plugin = plugins.GetPlugin(name);
            if (plugin != null && !plugin.IsOpen)
            {
                plugin.Open();
            }
        }

        /// <summary>
        /// Beendet alle Plugins
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var plugin in plugins.GetPlugins())
            {
                try
                {
                    plugin.Close();
                }
                catch (Exception x)
                {
                    Logger.Error(x, "Error closing plugin '{0}'.", plugin.Name);
                }
            }
        }
    }
}
