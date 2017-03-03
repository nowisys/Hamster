using System;
using System.Collections.Generic;
using Hamster.Plugin;
using Hamster.Plugin.Events;

namespace Hamster
{
	public class PluginDirectory : IPluginDirectory
	{
		private Dictionary<string, IPlugin> items;
        private IEventTrigger trigger;

		public PluginDirectory()
		{
			items = new Dictionary<string, IPlugin>( StringComparer.InvariantCultureIgnoreCase );
            trigger = new SimpleEventTrigger();
		}

        public event EventHandler<IPluginDirectory, PluginEventArgs> PluginRegistered;
        public event EventHandler<IPluginDirectory, PluginEventArgs> PluginUnregistered;

		public void Register( string name, IPlugin plugin )
		{
			lock( items )
			{
				if( items.ContainsKey( name ) )
				{
					throw new ArgumentException( "There is already a plugin registered with the name '" + name + "'.", "name" );
				}

				items[name] = plugin;
			}

            trigger.Invoke(PluginRegistered, this, new PluginEventArgs(plugin));
		}

		public bool Unregister( string name )
		{
            IPlugin plugin;
			lock( items )
			{
                if (items.TryGetValue(name, out plugin))
                {
                    items.Remove(name);
                }
			}

            if (plugin != null)
            {
                trigger.Invoke(PluginUnregistered, this, new PluginEventArgs(plugin));
                return true;
            }
            else
            {
                return false;
            }
		}

		public bool Contains( string name )
		{
			return items.ContainsKey( name );
		}

		public void Clear()
		{
			lock( items )
			{
				items.Clear();
			}
		}

		public IPlugin GetPlugin( string name )
		{
			IPlugin result;
			if( !items.TryGetValue( name, out result ) )
			{
				return null;
			}
			return result;
		}

		public IList<IPlugin> GetPlugins()
		{
			lock( items )
			{
				IPlugin[] result = new IPlugin[items.Count];
				items.Values.CopyTo( result, 0 );
				return result;
			}
		}
	}
}
