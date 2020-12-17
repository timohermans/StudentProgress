using System;
using NHibernate.Cfg;

namespace StudentProgress.Application.Migrations
{
    public class PostgreSqlNamingStrategy : INamingStrategy
    {
        public string ClassToTableName(string className)
        {
            return DoubleQuote(className);
        }

        public string PropertyToColumnName(string propertyName)
        {
            return DoubleQuote(propertyName);
        }

        public string TableName(string tableName)
        {
            return DoubleQuote(tableName);
        }

        public string ColumnName(string columnName)
        {
            return DoubleQuote(columnName);
        }

        public string PropertyToTableName(string className,
            string propertyName)
        {
            return DoubleQuote(propertyName);
        }

        public string LogicalColumnName(string columnName,
            string propertyName)
        {
            return String.IsNullOrWhiteSpace(columnName) ? DoubleQuote(propertyName) : DoubleQuote(columnName);
        }

        private static string DoubleQuote(string raw)
        {
            raw = raw.Replace("`", "");
            return $"\"{raw}\"";
        }
    }
}