using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Hamster.Plugin.Configuration
{
    public class PluginBind
    {
        [XmlAttribute("slot")]
        public string Slot { get; set; }

        [XmlAttribute("plugin")]
        public string Plugin { get; set; }
    }
}
