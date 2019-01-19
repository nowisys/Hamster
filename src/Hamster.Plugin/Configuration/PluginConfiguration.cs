using System;
using System.Xml;

namespace Hamster.Plugin.Configuration
{
    [Serializable()]
    public class PluginConfiguration
    {
        private string pluginName;
        private string assemblyName;
        private string typeName;
        private XmlElement settings;

        public PluginConfiguration()
        {

        }

        public string PluginName
        {
            get { return pluginName; }
            set { pluginName = value; }
        }

        public string AssemblyName
        {
            get { return assemblyName; }
            set { assemblyName = value; }
        }

        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        public XmlElement Settings
        {
            get { return settings; }
            set { settings = value; }
        }
    }
}
