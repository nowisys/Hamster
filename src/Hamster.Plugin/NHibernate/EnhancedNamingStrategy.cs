using NHibernate.Cfg;

namespace Hamster.Plugin.NHibernate
{
    public class EnhancedNamingStrategy : INamingStrategy
    {
        private string tablePrefix = string.Empty;
        private readonly INamingStrategy baseStrategy;

        public EnhancedNamingStrategy()
          : this(DefaultNamingStrategy.Instance)
        {

        }

        public string TablePrefix
        {
            get => tablePrefix;
            set => tablePrefix = value ?? string.Empty;
        }

        public EnhancedNamingStrategy(INamingStrategy baseStrategy)
        {
            this.baseStrategy = baseStrategy ?? DefaultNamingStrategy.Instance;
        }

        public string ClassToTableName(string className)
        {
            return tablePrefix + baseStrategy.ClassToTableName(className);
        }

        public string ColumnName(string columnName)
        {
            return baseStrategy.ColumnName(columnName);
        }

        public string PropertyToColumnName(string propertyName)
        {
            return baseStrategy.PropertyToColumnName(propertyName);
        }

        public string PropertyToTableName(string className, string propertyName)
        {
            return tablePrefix + baseStrategy.PropertyToTableName(className, propertyName);
        }

        public string TableName(string tableName)
        {
            return tablePrefix + baseStrategy.TableName(tableName);
        }

        public string LogicalColumnName(string columnName, string propertyName)
        {
            return baseStrategy.LogicalColumnName(columnName, propertyName);
        }
    }
}