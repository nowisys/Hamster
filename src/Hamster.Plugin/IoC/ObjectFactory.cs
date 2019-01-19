using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Hamster.Plugin
{
    /// <summary>
    /// Einfache implementierung von IObjectFactory, die für Tests
    /// oder einfache Anwendungen verwendet werden kann.
    /// </summary>
    public class ObjectFactory : IObjectFactory
    {
        private IServiceProvider services;

        public ObjectFactory(IServiceProvider services)
        {
            this.services = services;
        }

        public virtual object Construct(Type type, IReadOnlyDictionary<string, object> properties)
        {
            if (type == null) {
                throw new ArgumentNullException("type");
            }

            var ctors = from c in type.GetConstructors()
                orderby c.GetParameters().Length
                select c;

            var cache = new Dictionary<Type, object>();
            ConstructorInfo ctor = null;

            foreach (var c in ctors) {
                bool matched = true;

                foreach (var p in c.GetParameters()) {
                    if (properties != null && properties.ContainsKey(p.Name)) {
                        continue;
                    }

                    if (cache.ContainsKey(p.ParameterType)) {
                        continue;
                    }

                    if (services != null) {
                        var obj = services.GetService(p.ParameterType);
                        if (obj != null) {
                            cache[p.ParameterType] = obj;
                            continue;
                        }
                    }

                    matched = false;
                    break;
                }

                if (matched) {
                    ctor = c;
                }
            }

            if (ctor == null) {
                throw new ArgumentException($"No valid constructor for {type} found");
            }

            var parameters = ctor.GetParameters();
            var args = new object[parameters.Length];

            for (int i = 0; i < args.Length; ++i) {
                var p = parameters[i];

                if (properties == null || !properties.TryGetValue(p.Name, out args[i])) {
                    args[i] = cache[p.ParameterType];
                }
            }

            return ctor.Invoke(args);
        }

        public virtual void Connect(object obj, IReadOnlyDictionary<string, object> properties)
        {
            if (obj == null) {
                throw new ArgumentNullException("obj");
            }

            foreach (PropertyInfo prop in obj.GetType().GetProperties()) {
                object val;
                if (properties != null && properties.TryGetValue(prop.Name, out val)) {
                    prop.SetValue(obj, val, null);
                } else if (services != null) {
                    val = services.GetService(prop.PropertyType);
                    if (val != null) {
                        prop.SetValue(obj, val, null);
                    }
                }
            }
        }

        public virtual object Create(Type type, IReadOnlyDictionary<string, object> properties)
        {
            if (type == null) {
                throw new ArgumentNullException("type");
            }

            object obj = Construct(type, properties);
            Connect(obj, properties);
            return obj;
        }
    }
}
