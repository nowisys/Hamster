using System.Collections.Generic;
using Castle.Core.Logging;
using Castle.Windsor;
using Hamster.Plugin;
using Hamster.Log;
using System;

namespace Hamster
{
	public class CastleObjectFactory : IObjectFactory
	{
		private string name = string.Empty;
		public IWindsorContainer container;

		public CastleObjectFactory( IWindsorContainer container )
		{
			this.container = container;
		}

		protected CastleObjectFactory( string name, IWindsorContainer container )
		{
			this.name = name;
			this.container = container;
		}

		protected virtual string GetFullName( string name )
		{
			string fullName = name;
			if( !string.IsNullOrEmpty( this.name ) )
			{
				fullName = this.name + "." + name;
			}
			return fullName;
		}

        public object Create(string name, Type type, IDictionary<string, object> properties)
        {
            string fullName = GetFullName(name);

            container.AddComponent(name, type);

            System.Collections.Hashtable args = new System.Collections.Hashtable();
            foreach (KeyValuePair<string, object> prop in properties)
            {
                args[prop.Key] = prop.Value;
            }

            return container.Resolve(fullName, args);
        }

		public T Create<T>( string name, IDictionary<string, object> properties )
		{
            return (T)Create(name, typeof(T), properties);
		}

		public IObjectFactory GetSubFactory( string name )
		{
			return new CastleObjectFactory( GetFullName( name ), container );
		}

		public T GetService<T>()
		{
			return container.Resolve<T>();
		}

		public IList<T> GetServices<T>()
		{
			return container.Kernel.ResolveAll<T>();
		}
	}
}
