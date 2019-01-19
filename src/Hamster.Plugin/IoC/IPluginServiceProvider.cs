using System;
using System.Collections.Generic;
using System.Text;

namespace Hamster.Plugin.IoC
{
    public interface IPluginServiceProvider : IServiceProvider
    {
        Dictionary<Type, object> GetImplementations();
        List<object> GetServices();
        List<Type> GetServiceTypes();
    }
}
