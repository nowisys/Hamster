#if NHIBERNATE

using System;
using System.Linq;
using System.Collections.Generic;
using Hamster.Plugin.Configuration;
using Hamster.Plugin.NHibernate;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Type;

namespace Hamster
{
    public class NHibernatePluginManager : IPluginManager
    {
        private NHibernatePlugin database;
        private string pluginPrefix;

        public NHibernatePluginManager(NHibernatePlugin database)
        {
            this.database = database;
        }

        public string PluginPrefix
        {
            get { return pluginPrefix; }
            set { pluginPrefix = value; }
        }

        private ISession OpenSession()
        {
            return database.OpenSession();
        }

        protected virtual void SetPlugin(ISession session, PluginConfig config)
        {
            PluginConfiguration data = session.Get<PluginConfiguration>(config.Name);
            if (data == null)
                data = new PluginConfiguration() { PluginName = config.Name };

            int split = config.Type.IndexOf(',');
            data.TypeName = config.Type.Substring(0, split);
            data.AssemblyName = config.Type.Substring(split + 1);
            data.Settings = config.Settings;

            session.SaveOrUpdate(data);

            RemoveBindings(session, config.Name);

            foreach (PluginBind b in config.Bindings)
            {
                session.Save(new PluginBinding()
                {
                    TargetPlugin = config.Name,
                    NativeName = b.Slot,
                    BoundPlugin = b.Plugin
                });
            }
        }

        protected virtual void RemoveBindings(ISession session, string targetName)
        {
            session.Delete("select b from PluginBinding b where b.TargetPlugin = ?", targetName, NHibernateUtil.String);
        }

        #region IPluginManager Members

        public virtual IList<string> GetPluginNames()
        {
            using (ISession session = OpenSession())
            {
                return (from c in session.Linq<PluginConfiguration>()
                        select c.PluginName).ToArray();
            }
        }

        public virtual IList<PluginConfig> GetPlugins()
        {
            return (from name in GetPluginNames()
                    select GetPlugin(name)).ToArray();
        }

        public virtual PluginConfig GetPlugin(string name)
        {
            PluginConfiguration data;
            PluginBinding[] bindings;
            using (ISession session = OpenSession())
            {
                data = session.Get<PluginConfiguration>(name);
                if (data == null)
                    return null;

                bindings = (from b in session.Linq<PluginBinding>()
                            where b.TargetPlugin == name
                            select b).ToArray();
            }

            PluginConfig result = new PluginConfig()
            {
                Name = data.PluginName,
                Type = data.TypeName + ", " + data.AssemblyName,
                Settings = data.Settings,
                Bindings = Array.ConvertAll(bindings, x => new PluginBind() { Slot=x.NativeName, Plugin=x.BoundPlugin })
            };

            return result;
        }

        public virtual void AddPlugin(PluginConfig config)
        {
            using (ISession session = OpenSession())
            {
                PluginConfiguration data = session.Get<PluginConfiguration>(config.Name);
                if (data != null)
                    throw new ArgumentException("There is already a plugin with the name '{0}'.", config.Name);

                SetPlugin(session, config);
            }
        }

        public virtual void UpdatePlugin(PluginConfig config)
        {
            using (ISession session = OpenSession())
            {
                SetPlugin(session, config);
            }
        }

        public virtual void RemovePlugin(string name)
        {
            using (ISession session = OpenSession())
            {
                PluginConfiguration plugin = new PluginConfiguration();
                plugin.PluginName = name;
                session.Delete(plugin);

                RemoveBindings(session, name);

                session.Flush();
            }
        }

        #endregion
    }
}

#endif