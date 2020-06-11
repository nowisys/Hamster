using System.Xml.Serialization;
using Hamster.Plugin;

namespace Hamster.Configuration
{
    public class LogConfig
    {
        [XmlAttribute("directory")]
        public string Directory { get; set; }

        [XmlAttribute("level")]
        public LogLevel Level { get; set; }
    }
}
