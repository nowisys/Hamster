using System;
using System.Collections.Generic;

namespace Hamster
{
    class KernelServices : IServiceProvider
    {
        private Dictionary<Type, object> singletons = new Dictionary<Type, object>();
        private Dictionary<Type, Func<object>> factories = new Dictionary<Type, Func<object>>();

        public void AddSingleton(Type type, object service)
        {
            singletons[type] = service;
        }

        public void AddSingleton<T>(T service)
        {
            singletons[typeof(T)] = service;
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

        public object GetSingleton(Type type)
        {
            object service;
            if (singletons.TryGetValue(type, out service)) {
                return service;
            } else {
                return null;
            }
        }

        public object GetFromFactory(Type type)
        {
            Func<object> factory;
            if (factories.TryGetValue(type, out factory)) {
                return factory();
            } else {
                return null;
            }
        }

        public object GetService(Type type)
        {
            return GetSingleton(type) ?? GetFromFactory(type);
        }
    }
}
