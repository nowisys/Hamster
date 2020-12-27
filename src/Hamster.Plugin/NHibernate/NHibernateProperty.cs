using System;
using System.Xml.Serialization;

namespace Hamster.Plugin.NHibernate
{
    [Serializable()]
    public class NHibernateProperty
    {
        public NHibernateProperty()
        {

        }

        public NHibernateProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }


        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText()]
        public string Value { get; set; }
    }
}