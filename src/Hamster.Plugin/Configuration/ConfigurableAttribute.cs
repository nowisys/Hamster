using System;

namespace Hamster.Plugin.Configuration
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple=false, Inherited=false )]
    public class ConfigurableAttribute : Attribute
    {
        private Type configType;
        private string methodName;

        public ConfigurableAttribute( Type configType )
        {
            this.configType = configType;
        }

        public Type ConfigType
        {
            get { return configType; }
        }

        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }
    }
}
