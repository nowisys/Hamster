using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Hamster.Plugin.Configuration;
using Hamster.Properties;

namespace Hamster
{
    public class CastlePluginManager : IPluginManager
    {
        private string settingsFile = Path.Combine(Settings.Default.SettingsPath, "plugins.settings.xml");
        private string rootFile = Path.Combine(Settings.Default.SettingsPath, "plugins.xml");
        private string xmlSectionName = "xmlConfig";
        private string bindingSectionName = "bindings";
        private string pluginPrefix;

        private XmlWriterSettings writerSettings;

        public CastlePluginManager()
        {
            writerSettings = new XmlWriterSettings();
        }

        public string PluginPrefix
        {
            get { return pluginPrefix; }
            set { pluginPrefix = value; }
        }

        private XmlElement GetXmlSettings(string filename, string xpath)
        {
            if (!File.Exists(filename))
                return null;

            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            XmlElement element;
            if (!string.IsNullOrEmpty(xpath))
            {
                XmlNode node = doc.SelectSingleNode(xpath);
                element = node as XmlElement;
            }
            else
            {
                element = doc.DocumentElement;
            }

            return element;
        }

        private PluginConfig LoadPluginConfig(XmlElement component)
        {
            XmlElement configElement = component.SelectSingleNode(xmlSectionName) as XmlElement;

            string fullName = component.GetAttribute("type");

            PluginConfig result = new PluginConfig();
            result.Name = component.GetAttribute("id");
            result.Type = fullName;

            if (configElement != null)
            {
                result.Settings = GetXmlSettings(configElement.GetAttribute("file"), configElement.GetAttribute("xpath"));
            }

            var bindings = (XmlElement)component.SelectSingleNode(bindingSectionName);
            result.Bindings = GetBindings(bindings);

            return result;
        }

        private void SavePluginConfig(XmlElement component, PluginConfig config)
        {
            string qualifiedType = config.Type;
            if (component.GetAttribute("type") != qualifiedType)
            {
                throw new NotSupportedException("Changing the type of this plugin is not supported.");
            }

            XmlElement xmlConfigElement = component.SelectSingleNode(xmlSectionName) as XmlElement;
            if (xmlConfigElement == null)
            {
                xmlConfigElement = component.OwnerDocument.CreateElement(xmlSectionName);
                xmlConfigElement.SetAttribute("file", settingsFile);
                xmlConfigElement.SetAttribute("xpath", "/plugins/" + config.Name.Replace('.', '/'));
            }

            string file = xmlConfigElement.GetAttribute("file");
            string xpath = xmlConfigElement.GetAttribute("xpath");

            XmlElement settingsElement = GetXmlSettings(file, xpath);
            if (settingsElement == null)
            {
                throw new NotSupportedException("Automatically creating the settings location is not supported at the moment.");
            }
            else
            {
                settingsElement.ParentNode.ReplaceChild(settingsElement.OwnerDocument.ImportNode(config.Settings, true), settingsElement);
                settingsElement.OwnerDocument.Save(file);
            }

            XmlElement bindingElement = component.SelectSingleNode(bindingSectionName) as XmlElement;
            if (bindingElement != null)
            {
                bindingElement.RemoveAll();
            }
            else
            {
                bindingElement = component.OwnerDocument.CreateElement(bindingSectionName);
                component.AppendChild(bindingElement);
            }

            foreach (var bind in config.Bindings)
            {
                bindingElement.AppendChild(component.OwnerDocument.CreateElement(bind.Slot)).InnerText = bind.Plugin;
            }
        }

        private void SaveDocument(XmlDocument doc)
        {
            using (XmlWriter writer = XmlWriter.Create(rootFile, writerSettings))
            {
                doc.WriteTo(writer);
            }
        }

        private PluginBind[] GetBindings(XmlElement bindingSection)
        {
            var nodes = bindingSection.ChildNodes;

            XmlNode bindingNode;

            PluginBind[] result = new PluginBind[nodes.Count];
            for (int i = 0; i < nodes.Count; ++i)
            {
                bindingNode = nodes[i];

                result[i] = new PluginBind();
                result[i].Slot = bindingNode.LocalName;
                result[i].Plugin = bindingNode.InnerText;
            }
            return result;
        }

        #region IPluginManager Members
        public IList<string> GetPluginNames()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(rootFile);

            XmlNodeList nodes = doc.SelectNodes("/configuration/components/component/@id");

            List<string> result = new List<string>(nodes.Count);
            for (int i = 0; i < nodes.Count; ++i)
            {
                string name = nodes[i].InnerText;
                if (string.IsNullOrEmpty(pluginPrefix) || name.StartsWith(PluginPrefix))
                {
                    result.Add(name);
                }
            }
            return result;
        }

        public IList<PluginConfig> GetPlugins()
        {
            return (from name in GetPluginNames()
                    select GetPlugin(name)).ToArray();
        }

        public PluginConfig GetPlugin(string name)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(rootFile);

                XmlElement component = doc.SelectSingleNode(string.Format("/configuration/components/component[translate(@id,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{0}']", name.ToLowerInvariant())) as XmlElement;

                if (component == null)
                {
                    return null;
                }

                return LoadPluginConfig(component);
            }
            catch (Exception x)
            {
                throw;
            }
        }

        public void AddPlugin(PluginConfig config)
        {
            throw new NotSupportedException(string.Format("Adding this plugin is not supported. Please choose a name that does not start with '{0}'", pluginPrefix));
        }

        public void UpdatePlugin(PluginConfig config)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(rootFile);

            XmlElement component = doc.SelectSingleNode(string.Format("/configuration/components/component[translate(@id,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{0}']", config.Name.ToLowerInvariant())) as XmlElement;

            if (component != null)
            {
                SavePluginConfig(component, config);
                SaveDocument(doc);
            }
        }

        public void RemovePlugin(string name)
        {
            throw new NotSupportedException("Removing this plugin is not supported.");
        }

        #endregion
    }
}
