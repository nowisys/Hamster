using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Hamster.Plugin.Configuration
{
    public class FilePluginManager : IPluginManager
    {
        private string directory;
        private XmlSerializer serializer = new XmlSerializer(typeof(PluginConfig));
        private XmlWriterSettings settings;

        public FilePluginManager(string directory)
        {
            DirectoryInfo dir = new DirectoryInfo(directory);
            if (!dir.Exists)
                dir.Create();

            this.directory = dir.FullName;

            settings = new XmlWriterSettings()
            {
                Indent = true
            };
        }

        public string PluginPrefix { get; set; }

        protected virtual string GetPath(string pluginName)
        {
            return Path.Combine(directory, Path.GetFileName(pluginName) + ".xml");
        }

        protected virtual Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        protected virtual Stream OpenWrite(string path)
        {
            return File.Create(path);
        }

        protected virtual void Delete(string path)
        {
            lock (serializer)
            {
                FileInfo info = new FileInfo(path);
                if (info.Exists)
                {
                    info.Delete();
                }
            }
        }

        protected virtual PluginConfig GetFile(string path)
        {
            lock (serializer)
            {
                try
                {
                    using (Stream stream = OpenRead(path))
                    {
                        if (stream == null)
                            return null;

                        return (PluginConfig)serializer.Deserialize(stream);
                    }
                }
                catch (FileNotFoundException) {
                    return null;
                }
            }
        }

        protected virtual void SetFile(string path, PluginConfig config)
        {
            lock (serializer)
            {
                using (Stream stream = OpenWrite(path))
                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        serializer.Serialize(writer, config);
                        writer.Flush();
                    }
            }
        }

        public void AddPlugin(PluginConfig config)
        {
            lock (serializer)
            {
                string url = GetPath(config.Name);
                var data = GetFile(url);
                if (data != null)
                    throw new ArgumentException(string.Format("There is already a plugin with the name '{0}'.", config.Name));

                SetFile(url, config);
            }
        }

        public void UpdatePlugin(PluginConfig config)
        {
            SetFile(GetPath(config.Name), config);
        }

        public void RemovePlugin(string name)
        {
            Delete(GetPath(name));
        }

        public IList<PluginConfig> GetPlugins()
        {
            lock (serializer)
            {
                List<PluginConfig> result = new List<PluginConfig>();
                foreach (string name in GetPluginNames())
                {
                    result.Add(GetPlugin(name));
                }
                return result;
            }
        }

        public PluginConfig GetPlugin(string name)
        {
            return GetFile(GetPath(name));
        }

        public IList<string> GetPluginNames()
        {
            lock (serializer)
            {
                var files = Directory.GetFiles(directory, "*.xml", SearchOption.TopDirectoryOnly);
                var names = from x in files select Path.GetFileNameWithoutExtension(x);
                return names.ToList();
            }
        }
    }
}
