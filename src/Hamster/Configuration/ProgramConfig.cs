using System.Xml;
using System.Xml.Serialization;
using Hamster.Plugin.Configuration;

namespace Hamster.Configuration
{
    [XmlRoot("hamster")]
    public class ProgramConfig
    {
        [XmlElement("log")]
        public LogConfig Log { get; set; }

        [XmlElement("load")]
        public FilePatternConfig[] Assemblies { get; set; }

        [XmlElement("plugin", Namespace = "http://www.nowisys.de/hamster/schemas/plugin.xsd")]
        public PluginConfig[] Plugins { get; set; }
    }
}
