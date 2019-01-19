#if NHIBERNATE

using System;
using System.Collections.Generic;
using Hamster.Plugin.NHibernate;
using NHibernate;

namespace Hamster.Plugin.Sequence
{
    public class NHibernateSequenceProvider : ISequenceProvider
    {
        private NHibernatePlugin database;
        private ISequenceRule standardRule;
        private Dictionary<string, ISequenceRule> rules;
        private object syncRoot = new object();

        public NHibernateSequenceProvider(NHibernatePlugin database)
            : this(database, new Dictionary<string, ISequenceRule>())
        {

        }

        public NHibernateSequenceProvider(NHibernatePlugin database, Dictionary<string, ISequenceRule> rules)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }

            this.database = database;
            this.standardRule = new IncrementRule();
            this.rules = rules ?? new Dictionary<string, ISequenceRule>();
        }

        public ISequenceRule StandardRule
        {
            get { return standardRule; }
            set { standardRule = value ?? new IncrementRule(); }
        }

        public Dictionary<string, ISequenceRule> Rules
        {
            get { return rules; }
        }

        /// <summary>
        /// Liefert den Wert der Sequenz und setzt diese einen Schritt weiter.
        /// Falls die Sequenz nicht existiert, wird sie angelegt mit dem gegebenen Startwert.
        /// </summary>
        /// <param name="sequenceId">Bezeichnung der Sequenz.</param>
        /// <param name="startValue">Erster wert für die Sequenz.</param>
        /// <returns>Den Wert der Sequenz oder null falls die Sequenz nicht existiert.</returns>
        protected virtual int GetNext(ISession session, string sequenceId, int startValue)
        {
            int? result = GetNext(session, sequenceId);
            if (!result.HasValue)
            {
                result = startValue;
                SetSequence(session, sequenceId, startValue+1, null);
            }

            return result.Value;
        }

        protected virtual int? GetNext(ISession session, string sequenceId)
        {
            SequenceItem item = session.Get<SequenceItem>(sequenceId);
            if (item == null)
            {
                return null;
            }

            int result = item.Value;

            ISequenceRule rule;
            if (item.Rule == null || !rules.TryGetValue(item.Rule, out rule))
            {
                rule = standardRule;
            }
            item.Value = rule.GetNext(result);
            item.LastChange = DateTime.Now;

            session.Update(item);

            return result;
        }

        protected virtual void SetSequence(ISession session, string sequenceId, int value, string rule)
        {
            SequenceItem item = session.Get<SequenceItem>(sequenceId);
            if (item == null)
            {
                item = new SequenceItem();
                item.SequenceId = sequenceId;
                item.Value = value;
                item.Rule = rule;
                item.LastChange = DateTime.Now;
                session.Save(item);
            }
            else
            {
                item.Value = value;
                item.Rule = rule;
                item.LastChange = DateTime.Now;
                session.Update(item);
            }
        }

        protected virtual bool RemoveSequence(ISession session, string sequenceId)
        {
            SequenceItem item = session.Get<SequenceItem>(sequenceId);
            if (item != null)
            {
                session.Delete(item);
            }
            return item != null;
        }

        protected virtual SequenceItem GetSequence(ISession session, string sequenceId)
        {
            SequenceItem item = session.Get<SequenceItem>(sequenceId);
            return item;
        }

        protected virtual SequenceItem[] GetSequences(ISession session)
        {
            ICriteria query = session.CreateCriteria(typeof(SequenceItem));
            IList<SequenceItem> items = query.List<SequenceItem>();
            SequenceItem[] result = new SequenceItem[items.Count];
            items.CopyTo(result, 0);
            return result;
        }

        public int GetNext(string sequenceId, int startValue)
        {
            lock (syncRoot)
            {
                using (ISession session = database.OpenSession())
                {
                    int result = GetNext(session, sequenceId, startValue);
                    session.Flush();
                    return result;
                }
            }
        }

        public int? GetNext(string sequenceId)
        {
            lock (syncRoot)
            {
                using (ISession session = database.OpenSession())
                {
                    int? result = GetNext(session, sequenceId);
                    session.Flush();
                    return result;
                }
            }
        }

        public void SetSequence(string sequenceId, int value, string rule)
        {
            lock (syncRoot)
            {
                using (ISession session = database.OpenSession())
                {
                    SetSequence(session, sequenceId, value, rule);
                    session.Flush();
                }
            }
        }

        public bool RemoveSequence(string sequenceId)
        {
            lock (syncRoot)
            {
                using (ISession session = database.OpenSession())
                {
                    bool result = RemoveSequence(session, sequenceId);
                    session.Flush();
                    return result;
                }
            }
        }

        public SequenceItem GetSequence(string sequenceId)
        {
            lock (syncRoot)
            {
                using (ISession session = database.OpenSession())
                {
                    return GetSequence(session, sequenceId);
                }
            }
        }

        public SequenceItem[] GetSequences()
        {
            lock (syncRoot)
            {
                using (ISession session = database.OpenSession())
                {
                    return GetSequences(session);
                }
            }
        }
    }
}

#endif
