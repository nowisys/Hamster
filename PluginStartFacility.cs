using Castle.Core.Configuration;
using Castle.MicroKernel;
using Hamster.Plugin;
using Castle.MicroKernel.Facilities;

namespace Hamster
{
	public class PluginStartFacility : AbstractFacility
	{
		protected override void Init()
		{
			Kernel.ComponentRegistered += ComponentRegistered;
			Kernel.ComponentCreated += ComponentCreated;
		}

		protected void ComponentRegistered( string key, IHandler handler )
		{
			if( typeof( IPlugin ).IsAssignableFrom( handler.ComponentModel.Implementation ) )
			{
				handler.ComponentModel.Parameters.Add( "Name", handler.ComponentModel.Name );
			}
		}

		protected void ComponentCreated( Castle.Core.ComponentModel model, object instance )
		{
			IPlugin plugin = instance as IPlugin;
			if( plugin != null )
			{
				plugin.Init();
				plugin.Open();
			}
		}
	}
}
