using System.Xml;

namespace Hamster.Plugin.Configuration
{
    public interface IXmlConfigurable
    {
        void Configure( XmlElement element );
        bool IsConfigured { get; }
    }
}
