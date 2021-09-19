using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Xml.Serialization;
using Hamster.Plugin;
using Hamster.Plugin.Events;
using Hamster.Plugin.Debug;
using Hamster.Configuration;
using Hamster.Plugin.Configuration;

namespace Hamster
{
    public static class Program
    {
        private static ILogger logger;
        private static QueuingEventTrigger eventQueue;
        private static IPluginDirectory plugins;

        public static ProgramConfig LoadConfig(string path)
        {
            if (path == null)
            {
                logger.Info($"Load default configuration");
                return new ProgramConfig()
                {
                    Assemblies = new []
                        {new FilePatternConfig() {Path = "lib/hamster/plugins", Pattern = "*.dll", Recursive = true}},
                    Plugins = new []
                    {
                        new FilePatternConfig() {Path = "/etc/hamster/plugins", Pattern = "*.xml", Recursive = true},
                        new FilePatternConfig() {Path = "etc/hamster/plugins", Pattern = "*.xml", Recursive = true},
                    },
                };
            }
            else
            {
                logger.Info($"Load configuration from: {path}");
                var serializer = new XmlSerializer(typeof(ProgramConfig));
                using (var file = File.Open(path, FileMode.Open))
                {
                    return (ProgramConfig) serializer.Deserialize(file);
                }
            }
        }

        public static void LoadAssemblies(string path, string pattern, bool recurse)
        {
            path = Path.GetFullPath(path);
            if (!Directory.Exists(path))
            {
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
                    foreach (var type in assembly.DefinedTypes)
                    {
                        if (pluginType.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                        {
                            if (type.AssemblyQualifiedName != null)
                            {
                                var t = Type.GetType(type.AssemblyQualifiedName);
                            }
                        }
                    }

                    logger.Info($"Loaded assembly: {fullPath}");
                }
                catch (ReflectionTypeLoadException ex)
                {
                    if (ex.LoaderExceptions != null)
                    {
                        foreach (Exception inner in ex.LoaderExceptions)
                        {
                            logger.Warn(inner, $"Error loading assembly from: {fullPath}");
                        }
                    }
                }
                catch (Exception x)
                {
                    logger.Warn(x, $"Error loading assembly from: {fullPath}");
                }
            }
        }

        public static IServiceProvider GetServices(string name)
        {
            var services = new KernelServices();
            services.AddSingleton(logger.CreateChildLogger(name));
            services.AddSingleton<IObjectFactory>(new ObjectFactory(services));
            services.AddSingleton<IEventTrigger>(eventQueue);
            // TODO: Add more kernel services
            return services;
        }

        public static void LoadPlugins(List<PluginConfig> plugins, string path, string pattern, bool recurse)
        {
            path = Path.GetFullPath(path);
            if (!Directory.Exists(path))
            {
                logger.Info($"Skip plugins from {path}");
                return;
            }

            var serializer = new XmlSerializer(typeof(PluginConfig));

            logger.Info($"Load plugins from {path}");
            var option = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filePath in Directory.EnumerateFiles(path, pattern, option))
            {
                var fullPath = Path.GetFullPath(filePath);
                try
                {
                    using (var file = File.Open(fullPath, FileMode.Open))
                    {
                        var pluginConfig = (PluginConfig) serializer.Deserialize(file);
                        if(pluginConfig != null && !pluginConfig.Disabled) {
                            plugins.Add(pluginConfig);
                        }
                    }

                    logger.Info($"Loaded plugin config: {fullPath}");
                }
                catch (Exception x)
                {
                    logger.Warn(x, $"Error loading pluginc config from: {fullPath}");
                }
            }
        }

        public static List<PluginConfig> LoadAllPlugins(IEnumerable<FilePatternConfig> configs)
        {
            var plugins = new List<PluginConfig>();
            foreach (var pattern in configs)
            {
                LoadPlugins(plugins, pattern.Path, pattern.Pattern, pattern.Recursive);
            }

            plugins = SortPluginConfigs(plugins).ToList();
            return plugins;
        }

        public static void CreatePlugins(IEnumerable<PluginConfig> config)
        {
            foreach (var pluginConfig in config)
            {
                var services = GetServices(pluginConfig.Name);
                var factory = (IObjectFactory) services.GetService(typeof(IObjectFactory));
                var type = Type.GetType(pluginConfig.Type);
                var plugin = (IPlugin) factory.Create(type, new Dictionary<string, object>{{"Plugins", plugins}});
                plugin.Name = pluginConfig.Name;

                plugins.Register(plugin.Name, plugin);
            }
        }

        public static IList<PluginConfig> SortPluginConfigs(IList<PluginConfig> config)
        {
            var sortedConfig = new List<PluginConfig>();
            var adding = true;
            while (adding)
            {
                adding = false;
                var configToBeAdded = ((List<PluginConfig>) config).Where(x => !sortedConfig.Contains(x));
                foreach (var pluginConfig in configToBeAdded)
                {
                    adding = true;
                    if (pluginConfig.Bindings == null || pluginConfig.Bindings.Length == 0)
                    {
                        sortedConfig.Add(pluginConfig);
                        continue;
                    }

                    var bindingsSatisfied = true;
                    foreach (var pluginBinding in pluginConfig.Bindings)
                    {
                        if (config.FirstOrDefault(x => x.Name == pluginBinding.Plugin) == null)
                        {
                            logger.Warn($"Binding mismatch in plugin {pluginConfig.Name}. Unabled to find binding {pluginBinding.Plugin}!");
                        }
                        else
                        {
                            if (sortedConfig.FirstOrDefault(x => x.Name == pluginBinding.Plugin) == null)
                            {
                                bindingsSatisfied = false;
                            }
                        }
                    }

                    if (bindingsSatisfied)
                    {
                        sortedConfig.Add(pluginConfig);
                    }
                }
            }

            return sortedConfig;
        }

        public static void ConfigurePlugins(IEnumerable<PluginConfig> config)
        {
            foreach (var pluginConfig in config)
            {
                var plugin = plugins.GetPlugins().FirstOrDefault(x => x.Name == pluginConfig.Name);

                if (pluginConfig.Settings != null)
                {
                    var configurable = (IXmlConfigurable) plugin;
                    configurable?.Configure(pluginConfig.Settings);
                }

                if (pluginConfig.Bindings != null)
                {
                    var bindable = (IBindable) plugin;
                    if (bindable != null)
                    {
                        foreach (var binding in pluginConfig.Bindings)
                        {
                            bindable.Bind(binding.Slot, plugins.GetPlugins().FirstOrDefault(x => x.Name ==binding.Plugin));
                        }

                        bindable.BindingComplete();
                    }
                }

                plugin?.Init();
            }
        }

        private static string Locate(string path)
        {
            string prefix = Path.GetFullPath(".");
            while (prefix != null)
            {
                var result = Path.Combine(prefix, path);
                if (File.Exists(result))
                {
                    return result;
                }

                prefix = Path.GetDirectoryName(prefix);
            }

            return null;
        }

        public static void Main(string[] args)
        {
            var entry = Path.GetFullPath(Environment.GetCommandLineArgs()[0]);
            var prefix = Path.Combine(Path.GetDirectoryName(entry) ?? "", "../..");
            Directory.SetCurrentDirectory(prefix);

            logger = new DebugLogger("Hamster");
            eventQueue = new QueuingEventTrigger();
            plugins = new PluginDirectory();

            if (args.Length > 1)
            {
                logger.Fatal("Invalid command line arguments.");
                return;
            }

            string configPath = args.Length == 1 ? Path.GetFullPath(args[0]) : Locate("etc/hamster.xml");
            if (configPath != null && !File.Exists(configPath))
            {
                logger.Fatal($"Configuration file not found: {configPath}");
                return;
            }

            var config = LoadConfig(configPath);
            if (config.Plugins == null || config.Plugins.Length == 0)
            {
                logger.Fatal("No plugins configured");
                return;
            }

            if (config.Log != null)
            {
                logger.Info($"Switch to file logger: {config.Log.Directory}");
                logger = new FileLogger(config.Log.Directory, "Hamster", config.Log.Level);
            }

            try
            {

                foreach (var include in config.Assemblies)
                {
                    LoadAssemblies(include.Path, include.Pattern, include.Recursive);
                }

                var pluginConfigs = LoadAllPlugins(config.Plugins);
                CreatePlugins(pluginConfigs);
                ConfigurePlugins(pluginConfigs);

                foreach (var plugin in plugins.GetPlugins())
                {
                    plugin.Open();
                }

                eventQueue.RunForever();
            }
            catch (Exception e)
            {
                logger.Fatal(e, "Unhandled exception.");
            }
        }
    }
}