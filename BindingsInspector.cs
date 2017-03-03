using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using Hamster.Plugin.Configuration;
using System.Collections.Generic;

namespace Hamster
{
	public class BindingsInspector : IContributeComponentModelConstruction
	{
		private string sectionName;

		public BindingsInspector( string sectionName )
		{
			if( string.IsNullOrEmpty( sectionName ) )
			{
				this.sectionName = "bindings";
			}
			else
			{
				this.sectionName = sectionName;
			}
		}

		public virtual void ProcessModel( IKernel kernel, ComponentModel model )
		{
			if( model.Configuration != null )
			{
				IConfiguration bindings = model.Configuration.Children[sectionName];
				if( bindings != null )
				{
					bool bindable = typeof( IBindable ).IsAssignableFrom( model.Implementation );
					if( bindable )
					{
						SetupAsBindings( model, bindings );
					}
					else
					{
						SetupAsParameters( model, bindings );
					}
				}
			}
		}

		protected virtual void SetupAsBindings( ComponentModel model, IConfiguration bindings )
		{
			Dictionary<string, bool> dependencies = new Dictionary<string, bool>();
			DependencyModel dep;
			foreach( IConfiguration bind in bindings.Children )
			{
				if( !dependencies.ContainsKey( bind.Value ) )
				{
					dependencies[bind.Value] = true;
					dep = new DependencyModel( DependencyType.ServiceOverride, bind.Value, null, false );
					model.Dependencies.Add( dep );
				}
			}
		}

		protected virtual void SetupAsParameters( ComponentModel model, IConfiguration bindings )
		{
			Dictionary<string, bool> dependencies = new Dictionary<string, bool>();
			DependencyModel dep;
			foreach( IConfiguration bind in bindings.Children )
			{
				model.Parameters.Add( bind.Name, "${" + bind.Value + "}" );
				if( !dependencies.ContainsKey( bind.Value ) )
				{
					dependencies[bind.Value] = true;
					dep = new DependencyModel( DependencyType.ServiceOverride, bind.Value, null, false );
					model.Dependencies.Add( dep );
				}
			}
		}
	}
}
