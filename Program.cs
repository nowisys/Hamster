using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Castle.Core.Logging;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Hamster.Plugin;
using Hamster.Plugin.Configuration;
using Hamster.Properties;
using System.IO;

#if !SERVICE
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Text;
using Microsoft.Scripting;
#endif

namespace Hamster
{
	class Program
	{
		private static bool running;
        private static Hamster.Plugin.ILogger logger = Hamster.Plugin.NullLogger.Instance;
		private static WindsorContainer core;
		private static IPluginDirectory pluginDirectory;
		private static PluginLoader loader;
		private static ProcessManager processManager;

#if SERVICE
		private static void Main( string[] args )
		{
			ServiceBase[] servicesToRun;

			servicesToRun = new ServiceBase[] { new WindowsService() };

			ServiceBase.Run( servicesToRun );
		}
#else
		private static void Main( string[] args )
		{
			running = true;
			Console.CancelKeyPress += ConsoleCancel;
			
			Start();

			try
			{
                ScriptRuntime runtime = Python.CreateRuntime();
                ScriptEngine engine = runtime.GetEngine("python");
                runtime.LoadAssembly(typeof(IPlugin).Assembly);

                ScriptScope scope = engine.CreateScope();
                scope.SetVariable("core", core);
                scope.SetVariable("exit", new Action(() => running = false));
                scope.SetVariable("plugins", pluginDirectory);
                scope.SetVariable("log", core.Resolve<ILoggerManager>());
                scope.SetVariable("logread", (Action)delegate()
                {
                    var log = core.Resolve<ILoggerManager>().GetEntries()
                        .OrderBy(i => i.Time);
                    foreach (LogEventArgs entry in log)
                    {
                        Console.WriteLine("{0} {1} {2} - {3}", entry.Time, entry.Level, entry.LogName, entry.Message);
                    }
                });

                Console.Write(">> ");

                string line;
                StringBuilder text = new StringBuilder();
                while (running && (line = Console.ReadLine()) != null)
                {
                    if (text.Length > 0)
                        text.Append('\n');
                    text.Append(line);

                    if (text.Length == 0)
                    {
                        Console.Write(">> ");
                        continue;
                    }

                    ScriptSource source = engine.CreateScriptSourceFromString(text.ToString(), Microsoft.Scripting.SourceCodeKind.InteractiveCode);
                    switch (source.GetCodeProperties())
                    {
                        case ScriptCodeParseResult.IncompleteStatement:
                            if (string.IsNullOrEmpty(line))
                            {
                                goto default;
                            }
                            else
                            {
                                goto case ScriptCodeParseResult.IncompleteToken;
                            }

                        case ScriptCodeParseResult.IncompleteToken:
                            Console.Write(".. ");
                            break;

                        default:
                            try
                            {
                                source.Execute(scope);
                            }
                            catch (Exception x)
                            {
                                Console.WriteLine("Error: " + x.Message);
                                scope.SetVariable("__error__", x);
                            }

                            Console.Write(">> ");
                            text.Length = 0;
                            break;
                    }
                }
			}
			finally
			{
				Stop();
			}
        }

        private static void ConsoleCancel(object sender, ConsoleCancelEventArgs e)
        {
            running = false;
            e.Cancel = true;
        }
#endif

		public static void Start()
		{
            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;

			Environment.CurrentDirectory = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetEntryAssembly().Location );
			core = new WindsorContainer( new XmlInterpreter( new FileResource( Path.Combine(Settings.Default.SettingsPath, "core.xml") ) ) );

			ILoggerManager logManager = core.Resolve<ILoggerManager>();
            logger = logManager.GetLogger("Hamster");
			core.Kernel.Resolver.AddSubResolver( new HamsterLoggerResolver( logManager ) );

			pluginDirectory = core.Resolve<IPluginDirectory>();
			core.Kernel.ComponentCreated += new Castle.MicroKernel.ComponentInstanceDelegate( KernelComponentCreated );

			CastleObjectFactory factory = new CastleObjectFactory( core );
			core.Kernel.AddComponentInstance( "CoreFactory", typeof(IObjectFactory), factory );

			loader = core.GetService<PluginLoader>();
			loader.Parent = core;
			loader.LoadPluginAssemblies();
			
			loader.Start( pluginDirectory );

			processManager = core.GetService<ProcessManager>();
			processManager.Start();
		}

        private static void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Fatal((Exception)e.ExceptionObject, "Unhandled Exception!");
        }

		private static void KernelComponentCreated( Castle.Core.ComponentModel model, object instance )
		{
			IPlugin plugin = instance as IPlugin;
			if( plugin != null )
			{
				IPlugin current = pluginDirectory.GetPlugin( plugin.Name );
				if( current != plugin )
				{
					pluginDirectory.Register( plugin.Name, plugin );
				}
			}
		}

		public static void Stop()
		{
			processManager.Stop();
			loader.Stop();
		}
	}
}
