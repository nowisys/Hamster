using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Hamster.Plugin.NHibernate
{
    [Serializable()]
    [XmlRoot("settings", Namespace = "http://www.nowisys.de/hamster/plugins/services/nhibernate.xsd")]
    public class NHibernatePluginSettings
    {
        public NHibernatePluginSettings()
        {
            MappingAssemblies = new List<string>();
            MappingClasses = new List<Type>();
            MappingDirectories = new List<string>();
            MappingFiles = new List<string>();
            AttributeAssemblies = new List<string>();
            Properties = new Dictionary<string, string>();
        }

        [XmlElement("tablePrefix")]
        public string TablePrefix { get; set; }

        [XmlElement("file")]
        public List<string> MappingFiles { get; }

        [XmlElement("directory")]
        public List<string> MappingDirectories { get; }

        [XmlElement("assembly")]
        public List<string> MappingAssemblies { get; }

        [XmlElement("type")]
        public string[] MappingClassNames
        {
            get
            {
                var result = new string[MappingClasses.Count];
                for (var i = 0; i < result.Length; ++i)
                {
                    result[i] = MappingClasses[i].AssemblyQualifiedName;
                }
                return result;
            }
            set
            {
                MappingClasses.Clear();
                if (value == null)
                {
                    return;
                }
                foreach (var classname in value)
                {
                    MappingClasses.Add(Type.GetType(classname, true, false));
                }
            }
        }

        [XmlArray("properties")]
        [XmlArrayItem("prop")]
        public NHibernateProperty[] PropertyArray
        {
            get
            {
                var result = new NHibernateProperty[Properties.Count];
                var i = 0;
                foreach (var prop in Properties)
                {
                    result[i] = new NHibernateProperty(prop.Key, prop.Value);
                    i += 1;
                }
                return result;
            }
            set
            {
                Properties.Clear();
                if (value != null)
                {
                    foreach (var prop in value)
                    {
                        Properties.Add(prop.Name, prop.Value);
                    }
                }
            }
        }

        [XmlIgnore()]
        public List<Type> MappingClasses { get; }

        [XmlIgnore()]
        public Dictionary<string, string> Properties { get; }

        [XmlElement("attributeAssembly")]
        public List<string> AttributeAssemblies { get; set; }
    }
}