using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Xml.Serialization;

using Hamster.Plugin;
using Hamster.Plugin.Debug;
using Hamster.Configuration;
using Hamster.Plugin.Configuration;

namespace Hamster
{
    public class Program
    {
        private static ILogger logger;

        public static ProgramConfig LoadConfig(string path)
        {
            var serializer = new XmlSerializer(typeof(ProgramConfig));

            using (var file = File.Open(path, FileMode.Open))
            {
                return (ProgramConfig)serializer.Deserialize(file);
            }
        }

        public static void LoadAssemblies(string path, string pattern, bool recurse)
        {
            if (!Directory.Exists(path)) {
                logger.Info($"Skip assemblies from {path}");
                return;
            }

            logger.Info($"Load assemblies from {path}");
            var pluginType = typeof(IPlugin).GetTypeInfo();
            var option = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var file in Directory.EnumerateFiles(path, pattern, option))
            {
                var fullPath = Path.GetFullPath(file);
                logger.Info($"Load assembly: {fullPath}");
                try
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(fullPath);
                    Console.WriteLine(assembly.ToString());
                    foreach (var type in assembly.DefinedTypes) {
                        if (pluginType.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface) {
                            Console.WriteLine(type);
                            var t = Type.GetType(type.AssemblyQualifiedName);
                            Console.WriteLine(t);
                        }
                    }
                }
                catch (Exception x)
                {
                    logger.Warn(x, $"Error loading assemby from: {fullPath}");
                }
            }
        }

        public static IServiceProvider GetServices(string name)
        {
            var services = new KernelServices();
            services.AddSingleton<ILogger>(logger.CreateChildLogger(name));
            services.AddSingleton<IObjectFactory>(new ObjectFactory(services));
            // TODO: Add more kernel services
            return services;
        }

        public static Dictionary<string, IPlugin> CreatePlugins(IEnumerable<PluginConfig> config)
        {
            var plugins = new Dictionary<string, IPlugin>();

            foreach (var pluginConfig in config) {
                var services = GetServices(pluginConfig.Name);
                var factory = (IObjectFactory)services.GetService(typeof(IObjectFactory));
                var type = Type.GetType(pluginConfig.Type);
                var plugin = (IPlugin)factory.Create(type, new Dictionary<string, object>());
                plugin.Name = pluginConfig.Name;

                plugins[plugin.Name] = plugin;
            }

            return plugins;
        }

        public static void ConfigurePlugins(IDictionary<string, IPlugin> plugins, IEnumerable<PluginConfig> config)
        {
            foreach (var pluginConfig in config) {
                var plugin = plugins[pluginConfig.Name];

                if (pluginConfig.Settings != null) {
                    var configurable = (IXmlConfigurable)plugin;
                    configurable.Configure(pluginConfig.Settings);
                }

                if (pluginConfig.Bindings != null) {
                    var bindable = (IBindable)plugin;
                    if (bindable != null) {
                        foreach (var binding in pluginConfig.Bindings) {
                            bindable.Bind(binding.Slot, plugins[binding.Plugin]);
                        }
                    }
                    bindable.BindingComplete();
                }

                plugin.Init();
            }
        }

        public static void Main(string[] args)
        {
            logger = new DebugLogger("Hamster");
            var configPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "../etc/hamster.xml"));
            if (args.Length >= 1) {
                configPath = Path.GetFullPath(args[0]);
            }

            if (!File.Exists(configPath)) {
                logger.Fatal($"Configuration file not found: {configPath}");
                return;
            }

            var configDir = Path.GetDirectoryName(configPath);
            logger.Info($"Load configuration from: {configPath}");
            var config = LoadConfig(configPath);

            if (config.Plugins == null || config.Plugins.Length == 0) {
                logger.Fatal("No plugins configured");
                return;
            }

            if (config.Log != null)
            {
                logger.Info($"Switch to file logger: {config.Log.Directory}");
                logger = new FileLogger(config.Log.Directory, "Hamster", config.Log.Level);
            }

            try {
                Directory.SetCurrentDirectory(configDir);

                foreach (var include in config.Assemblies) {
                    LoadAssemblies(include.Path, include.Pattern, include.Recursive);
                }

                var plugins = CreatePlugins(config.Plugins);
                ConfigurePlugins(plugins, config.Plugins);

                foreach (var plugin in plugins.Values) {
                    plugin.Open();
                }

                Console.ReadKey();
            } catch (Exception e) {
                logger.Fatal(e, "Unhandled exception.");
            }
        }
    }
}
