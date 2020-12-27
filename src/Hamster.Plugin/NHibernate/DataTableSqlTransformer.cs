using System.Data;
using NHibernate.Engine.Query.Sql;
using NHibernate.Impl;

namespace Hamster.Plugin.NHibernate
{
    public class DataTableSqlTransformer : DataTableTransformer
    {
        private readonly SqlQueryImpl query;

        public DataTableSqlTransformer(DataTable table, SqlQueryImpl query)
          : base(table, query)
        {
            this.query = query;
        }

        protected override void BuildTable(DataTable table, string[] aliases)
        {
            var returns = query.GenerateQuerySpecification(null).SqlQueryReturns;
            foreach (var t in returns)
            {
                if (t is NativeSQLQueryScalarReturn ret)
                {
                    table.Columns.Add(ret.ColumnAlias, ret.Type.ReturnedClass);
                }
            }
        }
    }
}
