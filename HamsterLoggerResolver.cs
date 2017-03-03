using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Hamster.Plugin;
using System;

namespace Hamster
{
	public class HamsterLoggerResolver : ISubDependencyResolver
	{
		private ILoggerManager manager;

		public HamsterLoggerResolver( ILoggerManager manager )
		{
			if( manager == null )
			{
				throw new ArgumentNullException();
			}

			this.manager = manager;
		}

		public bool CanResolve( CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency )
		{
			return dependency.TargetType == typeof( ILogger );
		}

		public object Resolve( CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency )
		{
			if( CanResolve( context, parentResolver, model, dependency ) )
			{
				return manager.GetLogger( model.Name );
			}

			return null;
		}
	}
}
