using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Hamster.Plugin.Configuration
{
    public class ResourceHelper
    {
        public ResourceHelper(IAdminHost target, object plugin)
            : this(target, plugin.GetType())
        {
        }

        public ResourceHelper(IAdminHost target, Type pluginType)
            : this(target, pluginType.GetTypeInfo().Assembly)
        {
            this.Namespace = pluginType.Namespace;
        }

        public ResourceHelper(IAdminHost target, Assembly source)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (source == null)
                throw new ArgumentNullException("source");

            this.Source = source;
            this.Namespace = source.GetName().Name;
            this.Target = target;
        }

        public string Namespace { get; set; }
        public Assembly Source { get; private set; }
        public IAdminHost Target { get; private set; }

        protected virtual void AddResource(string dest, string name, string resource)
        {
            using (Stream data = Source.GetManifestResourceStream(resource))
            {
                Target.AddResource(dest + name, data);
            }
        }

        public virtual void AddRegex(string dest, Regex expr)
        {
            int group = expr.GroupNumberFromName("name");
            if (group < 0)
                group = 0;

            foreach (string res in Source.GetManifestResourceNames())
            {
                var name = res;
                if (Namespace != null)
                {
                    if (!name.StartsWith(Namespace))
                        continue;

                    name = name.Substring(Namespace.Length);
                    name = name.TrimStart('.');
                }

                var match = expr.Match(name).Groups[group];
                if (match.Success)
                {
                    AddResource(dest, res, match.Value);
                }
            }
        }

        public virtual void AddDir(string dest, string dir)
        {
            dir = dir.Replace('/', '.');
            dir = dir.Replace('\\', '.');
            dir.TrimEnd('.');

            string expr = string.Concat("^", Regex.Escape(dir), @"\.(?<name>.*)$");
            AddRegex(dest, expr);
        }

        public virtual void AddFile(string dest, string name)
        {
            string res = string.Concat(Namespace, ".", name);
            AddResource(dest, name, res);
        }

        public virtual void AddGlob(string dest, string pattern)
        {
            var expr = Glob.CreateRegex(pattern, true);
            AddRegex(dest, expr);
        }

        public virtual void AddRegex(string dest, string pattern)
        {
            var expr = new Regex(pattern, RegexOptions.Compiled);
            AddRegex(dest, expr);
        }
    }
}
