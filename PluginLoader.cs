using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Castle.Core.Configuration;
using Castle.Core.Resource;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Installer;
using Hamster.Plugin;
using Hamster.Plugin.Configuration;
using Hamster.Properties;

namespace Hamster
{
	public class PluginLoader : IDisposable
	{
		private Castle.Core.Logging.ILogger logger;
		private IWindsorContainer parent;
		private IWindsorContainer pluginContainer;
		private IPluginDirectory plugins;
        private IPluginManager switchManager;
		private string[] startupPlugins;
		private Queue<string> startupList;
		private string pluginSearchPath = Hamster.Properties.Settings.Default.PluginPath;
		private bool loadRecursive = true;
		private Dictionary<string, Assembly> partialNameLookup = new Dictionary<string, Assembly>();
		private Dictionary<string, Assembly> qualifiedNameLookup = new Dictionary<string, Assembly>();

		private AutoResetEvent initLock = new AutoResetEvent( false );

		public PluginLoader( IPluginManager manager )
		{
            this.switchManager = manager;
			this.startupList = new Queue<string>();
			logger = Castle.Core.Logging.NullLogger.Instance;
		}

		public Castle.Core.Logging.ILogger Logger
		{
			get { return logger; }
			set { logger = value ?? Castle.Core.Logging.NullLogger.Instance; }
		}

		public string[] StartupPlugins
		{
			get { return startupPlugins; }
			set { startupPlugins = value ?? new string[0]; }
		}

		public IWindsorContainer Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public string PluginSearchPath
		{
			get { return pluginSearchPath; }
			set { pluginSearchPath = value; }
		}

		public bool LoadRecursive
		{
			get { return loadRecursive; }
			set { loadRecursive = value; }
		}

		public virtual void LoadPluginAssemblies()
		{
			if( Directory.Exists( PluginSearchPath ) )
			{
				SearchOption option = loadRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
				Assembly asm;
				AssemblyName name;
				foreach( string file in Directory.GetFiles( PluginSearchPath, "*.dll", option ) )
				{
					try
					{
						asm = Assembly.LoadFrom( file );
						name = asm.GetName();

                        logger.Info("Load assembly '{0}'.", name.FullName);

						if( qualifiedNameLookup.ContainsKey( name.FullName ) )
						{
							logger.Error( "The assembly '{0}' has already been loaded from a different location.", name.FullName );
						}
						else
						{
							qualifiedNameLookup[name.FullName] = asm;

							if( partialNameLookup.ContainsKey( name.Name ) )
							{
                                string message = "An assembly with the name '{0}' has already been loaded. This is probably an error.\n    First: {1}\n    Second: {2}";
                                logger.WarnFormat(message, name.Name, partialNameLookup[name.Name].Location, asm.Location);
							}
							else
							{
                                partialNameLookup[name.Name] = asm;
							}
						}
					}
					catch( Exception x )
					{
						logger.ErrorFormat( x, "Error while loading '{0}'.", file );
					}
				}
			}
			AppDomain.CurrentDomain.AssemblyResolve += ResolvePluginAssembly;
		}

		protected virtual Assembly ResolvePluginAssembly( object sender, ResolveEventArgs args )
		{
			Assembly result;
			if( qualifiedNameLookup.TryGetValue( args.Name, out result ) || partialNameLookup.TryGetValue( args.Name, out result ) )
			{
				return result;
			}
			else
			{
				return null;
			}
		}

		public virtual void Start( IPluginDirectory directory )
		{
			if( directory == null )
			{
				throw new ArgumentNullException( "directory" );
			}

			logger.Info( "Initialize plugin loader." );

			this.plugins = directory;

			plugins.Clear();

			pluginContainer = new WindsorContainer();
			pluginContainer.Kernel.AddComponentInstance( "container", typeof( IWindsorContainer ), pluginContainer );
			pluginContainer.Kernel.ComponentRegistered += KernelComponentRegistered;

			if( parent != null )
			{
				logger.Debug( "Set parent container." );
				parent.AddChildContainer( pluginContainer );
				parent.Kernel.AddComponentInstance( "PluginContainer", typeof( IWindsorContainer ), pluginContainer );
				ILoggerManager logManager = pluginContainer.Resolve<ILoggerManager>();
				pluginContainer.Kernel.Resolver.AddSubResolver( new HamsterLoggerResolver( logManager ) );
				CastleObjectFactory factory = new CastleObjectFactory( pluginContainer );
				pluginContainer.Kernel.AddComponentInstance( "PluginFactory", typeof( IObjectFactory ), factory );
			}

			logger.Debug( "Create resource interpreter." );
			XmlInterpreter interpreter = new XmlInterpreter( new FileResource( Path.Combine(Settings.Default.SettingsPath, "plugins.xml") ) );
			interpreter.ProcessResource( interpreter.Source, pluginContainer.Kernel.ConfigurationStore );

			logger.Debug( "Setup components." );
			DefaultComponentInstaller installer = new Castle.Windsor.Installer.DefaultComponentInstaller();
			installer.SetUp( pluginContainer, pluginContainer.Kernel.ConfigurationStore );

			ThreadPool.RegisterWaitForSingleObject( initLock, InitializePlugins, null, 5*1000, true );

			logger.Info( "Initialized plugin loader." );
		}

		protected virtual void InitializePlugins( object state, bool timedOut )
		{
			if( !timedOut )
			{
				// Falls kein Timeout vorliegt wurde das initLock gesetzt,
				// was bedeutet der Pluginloader wurde geschlossen.
				// Macht keinen Sinn dann noch Plugins laden zu wollen.
				return;
			}

			try
			{
				logger.Info( "Loading plugins." );
				LoadPlugins();
			}
			catch( Exception x )
			{
				logger.Error( "An error occured while loading plugins. Retry again later.", x );
				ThreadPool.RegisterWaitForSingleObject( initLock, InitializePlugins, null, 5*60*1000, true );
			}

			logger.Info( "Starting new loaded plugins..." );
			StartPlugins();
			logger.Info( "Started new loaded plugins." );
		}

		protected virtual void KernelComponentRegistered( string key, Castle.MicroKernel.IHandler handler )
		{
			if( typeof( IPlugin ).IsAssignableFrom( handler.ComponentModel.Implementation ) )
			{
				lock( startupList )
				{
					Logger.Info( "Registering '{0}' for startup.", key );
					startupList.Enqueue( key );
				}
			}
			else
			{
				Type type = handler.ComponentModel.Implementation.GetInterface( "Hamster.Plugin.IPlugin" );
				if( type != null )
				{
					Logger.WarnFormat( "Plugin interface mismatch with component '{0}'.", key );
				}
			}
		}

		protected virtual void StartPlugins()
		{
			string pluginName;
			lock( startupList )
			{
				while( startupList.Count > 0 )
				{
					pluginName = startupList.Dequeue();

					try
					{
						Logger.InfoFormat( "Starting plugin '{0}'.", pluginName );
						IPlugin plugin = pluginContainer.Resolve<IPlugin>( pluginName );
						plugins.Register( pluginName, plugin );
						Logger.InfoFormat( "Started plugin '{0}'.", pluginName );
					}
					catch( Exception x )
					{
						Logger.Error( string.Format( "Error while loading plugin '{0}'.", pluginName ), x );
					}
				}
			}
		}

		protected virtual void LoadPlugins()
		{
			DefaultConfigurationStore pluginConfigurations = new DefaultConfigurationStore();

			for( int i = 0; i < startupPlugins.Length; ++i )
			{
				if( startupPlugins[i] == null )
					continue;

				IPluginManager manager;
				IList<PluginConfig> configItems;

				try
				{
					Logger.InfoFormat( "Loading configurations from '{0}'.", startupPlugins[i] );
					manager = pluginContainer.Resolve<IPluginManager>( startupPlugins[i] );
					configItems = manager.GetPlugins();
				}
				catch( Exception x )
				{
					Logger.ErrorFormat( x, "Error while loading plugins from '{0}'.", startupPlugins[i] );
					throw;
				}

				startupPlugins[i] = null;

				foreach( var config in configItems )
				{
                    if (config.Disabled)
                    {
                        Logger.Info("Plugin '{0}' is disabled.", config.Name);
                        continue;
                    }

					try
					{
						Logger.InfoFormat( "Loading configuration for plugin '{0}'.", config.Name );
                        IList<PluginBind> bindings = config.Bindings;

						IConfiguration castleConfig = CreateCastleConfiguration( config, bindings );
						pluginConfigurations.AddComponentConfiguration( config.Name, castleConfig );
                        pluginContainer.Kernel.ConfigurationStore.AddComponentConfiguration(config.Name, castleConfig);
					}
					catch( Exception x )
					{
                        Logger.ErrorFormat(x, "Error while loading plugin '{0}'.", config.Name);
					}
				}
			}

			DefaultComponentInstaller installer = new DefaultComponentInstaller();
			installer.SetUp( pluginContainer, pluginConfigurations );
		}

		protected virtual IConfiguration CreateCastleConfiguration( PluginConfig config, IList<PluginBind> bindings )
		{
			MutableConfiguration result = new MutableConfiguration( "component" );
			result.Attribute( "id", config.Name );
			result.Attribute( "type", config.Type );

			if( config.Settings != null )
			{
				string text = config.Settings.OuterXml;
				MutableConfiguration xmlConfig = result.CreateChild( "xmlConfig" );
				xmlConfig.Attribute( "xml", text );
			}

			if( bindings != null && bindings.Count > 0 )
			{
				MutableConfiguration bindConfig = result.CreateChild( "bindings" );
				foreach( var bind in bindings )
				{
					bindConfig.CreateChild( bind.Slot, bind.Plugin );
				}
			}

			return result;
		}

		public virtual void Stop()
		{
			((IDisposable)this).Dispose();
		}

		protected virtual void Dispose( bool disposing )
		{
			if( disposing )
			{
				initLock.Set();
				pluginContainer.Dispose();
			}
		}

		#region IDisposable Pattern

		void IDisposable.Dispose()
		{
			GC.SuppressFinalize( this );
			Dispose( true );
		}

		~PluginLoader()
		{
			Dispose( false );
		}

		#endregion
	}
}
