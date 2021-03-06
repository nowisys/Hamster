using System.Xml.Serialization;

namespace Hamster.Configuration
{
    [XmlRoot("hamster")]
    public class ProgramConfig
    {
        [XmlElement("log")]
        public LogConfig Log { get; set; }

        [XmlElement("load")]
        public FilePatternConfig[] Assemblies { get; set; }

        [XmlElement("plugins")]
        public FilePatternConfig[] Plugins { get; set; }
    }
}
