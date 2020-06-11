using System.Xml.Serialization;

namespace Hamster.Configuration
{
    public class FilePatternConfig
    {
        [XmlAttribute("path")]
        public string Path { get; set; }

        [XmlAttribute("pattern")]
        public string Pattern { get; set; }

        [XmlAttribute("recursive")]
        public bool Recursive { get; set; }
    }
}
