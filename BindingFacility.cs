using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;
using Hamster.Plugin.Configuration;
using System;
using Castle.MicroKernel.Facilities;
using Hamster.Plugin;

namespace Hamster
{
	public class BindingFacility : AbstractFacility
	{
		private string sectionName;
		private string loggerName;
		private BindingsInspector inspector;

		public BindingFacility()
		{
			
		}

		protected override void Init()
		{
			sectionName = FacilityConfig.Attributes["sectionName"];
			if( string.IsNullOrEmpty( sectionName ) )
			{
				sectionName = "bindings";
			}

			loggerName = FacilityConfig.Attributes["loggerName"];
			if( string.IsNullOrEmpty( loggerName ) )
			{
				loggerName = "Hamster.Bindings";
			}

			Kernel.AddComponent( loggerName, typeof( LoggerProxy ), LifestyleType.Transient );

			inspector = new BindingsInspector( sectionName );
			Kernel.ComponentModelBuilder.AddContributor( inspector );

			Kernel.ComponentCreated += KernelComponentCreated;
		}

		protected virtual void KernelComponentCreated( ComponentModel model, object instance )
		{
			if( model.Name == loggerName )
				return;

			LoggerProxy loggerProxy = (LoggerProxy)Kernel.Resolve( loggerName, typeof( LoggerProxy ) );

			IBindable bindable = instance as IBindable;
			if( bindable != null )
			{
				try
				{
					IConfiguration bindings = model.Configuration.Children[sectionName];
					if( bindings != null )
					{
						object boundObject;
						foreach( IConfiguration bind in bindings.Children )
						{
							boundObject = Kernel.Resolve( bind.Value, new System.Collections.Hashtable() );
							bindable.Bind( bind.Name, boundObject );
						}
					}
					bindable.BindingComplete();
				}
				catch( Exception x )
				{
					loggerProxy.Logger.Error( x, "Error while binding to '{0}'.", model.Name );
					throw;
				}
			}
		}
	}
}
