using System;
using System.Collections;
using System.Data;
using NHibernate;
using NHibernate.Transform;

namespace Hamster.Plugin.NHibernate
{
    public class DataTableTransformer : IResultTransformer
    {
        private readonly DataTable table;
        private readonly IQuery query;

        public DataTableTransformer(DataTable table, IQuery query)
        {
            this.table = table ?? throw new ArgumentNullException(nameof(table));
            this.query = query ?? throw new ArgumentNullException(nameof(query));
        }

        protected virtual void BuildTable(DataTable dtable, string[] aliases)
        {
            var types = query.ReturnTypes;
            for (var i = 0; i < types.Length; ++i)
            {
                dtable.Columns.Add(aliases[i], types[i].ReturnedClass);
            }
        }

        public virtual IList TransformList(IList collection)
        {
            return collection;
        }

        public virtual object TransformTuple(object[] tuple, string[] aliases)
        {
            if (aliases == null)
                throw new NotSupportedException("The transformer does not support queries without alias.");

            if (table.Columns.Count == 0)
                BuildTable(table, aliases);

            var result = table.NewRow();
            for (var i = 0; i < aliases.Length; ++i)
            {
                if (table.Columns.Contains(aliases[i]))
                {
                    result[aliases[i]] = tuple[i] ?? DBNull.Value;
                }
            }

            return result;
        }
    }
}
