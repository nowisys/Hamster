using System.Collections.Generic;
using System;

namespace Hamster.Plugin
{
    public interface IObjectFactory
    {
        object Construct(Type type, IReadOnlyDictionary<string, object> properties);
        void Connect(object obj, IReadOnlyDictionary<string, object> properties);
        object Create(Type type, IReadOnlyDictionary<string, object> properties);
    }
}
