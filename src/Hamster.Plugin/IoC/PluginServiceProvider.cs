using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamster.Plugin.IoC
{
    public class PluginServiceProvider : IPluginServiceProvider
    {
        private Dictionary<Type, object> services = new Dictionary<Type, object>();
        private Dictionary<Type, Func<object>> factories = new Dictionary<Type, Func<object>>();

        public void AddSingleton(Type type, object service)
        {
            services[type] = service;
        }

        public void AddSingleton<T>(T service)
        {
            services[typeof(T)] = service;
        }

        public void AddFactory(Type type, Func<object> factory)
        {
            factories[type] = factory;
        }

        public void AddFactory<T>(Func<T> factory)
            where T : class
        {
            factories[typeof(T)] = factory;
        }

        public object GetFromFactory(Type type)
        {
            Func<object> factory;
            if (factories.TryGetValue(type, out factory))
            {
                return factory();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Searches in provided services and factories for a given type.
        /// </summary>
        /// <param name="type">Type to search for.</param>
        /// <returns>Service for a given type. Null if Type was not found.</returns>
        public object GetService(Type type)
        {
            object service;
            if (services.TryGetValue(type, out service))
            {
                return service;
            }
            else
            {
                return GetFromFactory(type);
            }
        }

        public List<object> GetServices()
        {
            return services.Values.ToList();
        }

        public List<Type> GetServiceTypes()
        {
            return services.Keys.ToList();
        }

        public Dictionary<Type, object> GetImplementations()
        {
            return services;
        }
    }
}
