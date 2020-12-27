using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Hamster.Plugin.Configuration;
using NHibernate;
using NHibernate.Impl;
using Array = System.Array;

namespace Hamster.Plugin.NHibernate
{
    [Configurable(typeof(NHibernatePluginSettings))]
    public class NHibernatePlugin : AbstractPlugin<NHibernatePluginSettings>
    {
        private ISessionFactory factory;

        /// <summary>
        /// Führt eine Abfrage durch und gibt das Ergebnis als DataTable zurück.
        /// </summary>
        /// <param name="session">ISession in der die Abfrage ausgeführt wird.</param>
        /// <param name="name">Name der Abfrage.</param>
        /// <param name="parameters">Parameter für die Abfrage.</param>
        /// <returns>Ergebnis der Abfrage.</returns>
        public virtual DataTable DynamicQuery(ISession session, string name, IDictionary<string, object> parameters)
        {
            var result = new DataTable(name);

            var query = session.GetNamedQuery(name) as SqlQueryImpl;
            if (query == null)
                throw new NotSupportedException($"Die angegebene Abfrage '{name}' ist ungültig. Es muss der Name einer native SQL-Abfrage angegeben werden.");

            foreach (var param in parameters)
            {
                if (Array.IndexOf(query.NamedParameters, param.Key) != -1)
                {
                    query.SetParameter(param.Key, param.Value);
                }
            }

            query.SetResultTransformer(new DataTableSqlTransformer(result, query));

            foreach (DataRow row in query.List())
            {
                result.Rows.Add(row);
            }

            return result;
        }

        /// <summary>
        /// Führt eine Abfrage durch und gibt das Ergebnis als DataTable zurück.
        /// </summary>
        /// <param name="session">IStatelessSession in der die Abfrage ausgeführt wird.</param>
        /// <param name="name">Name der Abfrage.</param>
        /// <param name="parameters">Parameter für die Abfrage.</param>
        /// <returns>Ergebnis der Abfrage.</returns>
        public virtual DataTable DynamicQuery(IStatelessSession session, string name, IDictionary<string, object> parameters)
        {
            var result = new DataTable(name);

            var query = session.GetNamedQuery(name) as SqlQueryImpl;
            if (query == null)
                throw new NotSupportedException($"Die angegebene Abfrage '{name}' ist ungültig. Es muss der Name einer native SQL-Abfrage angegeben werden.");

            foreach (var param in parameters)
            {
                if (Array.IndexOf(query.NamedParameters, param.Key) != -1)
                {
                    query.SetParameter(param.Key, param.Value);
                }
            }

            query.SetResultTransformer(new DataTableSqlTransformer(result, query));

            foreach (DataRow row in query.List())
            {
                result.Rows.Add(row);
            }

            return result;
        }

        /// <summary>
        /// Öffnet eine neue Session auf der Datenbank.
        /// </summary>
        /// <returns>ISession auf der Datenbank.</returns>
        public virtual ISession OpenSession()
        {
            using (GetStateLock(false))
            {
                if (!IsOpen)
                    throw new InvalidOperationException("Cannot open session while plugin is not opened.");

                return factory.OpenSession();
            }
        }

        /// <summary>
        /// Öffnet eine neue Session ohne Cache auf der Datenbank.
        /// </summary>
        /// <remarks>
        /// Eine IStatelessSession eignet sich besonders für Massentransaktionen,
        /// da dort der Cache wenig nützt aber viel Zeit zur Pfege braucht.
        /// </remarks>
        /// <returns>IStatelessSession auf der Datenbank.</returns>
        public virtual IStatelessSession OpenStatelessSession()
        {
            using (GetStateLock(false))
            {
                if (!IsOpen)
                    throw new InvalidOperationException("Cannot open session while plugin is not opened.");

                return factory.OpenStatelessSession();
            }
        }

        protected virtual ISessionFactory CreateFactory()
        {
            Logger.Info("Loading Configuration...");

            try
            {
                var config = new global::NHibernate.Cfg.Configuration();

                var strat = new EnhancedNamingStrategy
                {
                    TablePrefix = Settings.TablePrefix
                };
                config.SetNamingStrategy(strat);

                config.SetProperties(Settings.Properties);
                foreach (var directory in Settings.MappingDirectories)
                {
                    var dir = new DirectoryInfo(directory);
                    config.AddDirectory(dir);
                }
                foreach (var file in Settings.MappingFiles)
                {
                    config.AddFile(file);
                }
                foreach (var assembly in Settings.MappingAssemblies)
                {
                    config.AddAssembly(assembly);
                }

                return config.BuildSessionFactory();
            }
            catch (Exception x)
            {
                Logger.Error(x, "Error while loading configuration.");
                throw;
            }
            finally
            {
                Logger.Info("Configuration successfully loaded.");
            }
        }

        protected override void BaseOpen()
        {
            factory = CreateFactory();
        }

        protected override void BaseClose()
        {
            factory.Dispose();
        }
    }
}