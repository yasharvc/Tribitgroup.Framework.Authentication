using System.Linq.Expressions;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder
{
    public class Column
    {
        public string TableName { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;

        public override string ToString() => $"{TableName}.{ColumnName}{(string.IsNullOrEmpty(Alias) ? "" : $" as {Alias}")}";

        public static Column From<T, ID>(Expression<Func<T, object>> propertySelector, string alias = "") where T : IEntity<ID>, new() where ID : notnull
        {
            var obj = new T();
            var res = new Column
            {
                TableName = obj.GetTableName(),
                Alias = alias,
                ColumnName = obj.SelectProperties(propertySelector).First().Name
            };
            return res;
        }

        public static Column From<T>(Expression<Func<T, object>> propertySelector, string alias = "") where T : IEntity<Guid>, new() => From<T, Guid>(propertySelector, alias);
    }
}
