using System.Xml;
using System.Xml.Serialization;

namespace Hamster.Plugin.Configuration
{
    [XmlRoot("plugin", Namespace="http://www.nowisys.de/hamster/schemas/plugin.xsd")]
    public class PluginConfig
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("disabled")]
        public bool Disabled { get; set; }

        [XmlElement("bind")]
        public PluginBind[] Bindings { get; set; }

        [XmlAnyElement()]
        public XmlElement Settings { get; set; }
    }
}
