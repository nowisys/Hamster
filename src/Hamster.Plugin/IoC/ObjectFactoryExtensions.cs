using System;
using System.Reflection;
using System.Collections.Generic;

namespace Hamster.Plugin
{
    public static class ObjectFactoryExtensions
    {
        static public Dictionary<string, object> ToDict(this object obj)
        {
            var props = new Dictionary<string, object>();
            foreach(var p in obj.GetType().GetProperties()) {
                props[p.Name] = p.GetValue(obj);
            }
            return props;
        }

        static public T Construct<T>(this IObjectFactory factory, IReadOnlyDictionary<string, object> properties)
        {
            return (T)factory.Create(typeof(T), properties);
        }

        static public T Create<T>(this IObjectFactory factory, IReadOnlyDictionary<string, object> properties)
        {
            return (T)factory.Create(typeof(T), properties);
        }
    }
}
