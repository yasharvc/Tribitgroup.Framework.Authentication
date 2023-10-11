using System.Linq.Expressions;
using Tribitgroup.Framework.Shared.Extensions;

namespace Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder
{
    public class QueryMapperInfo<TDTO> where TDTO : class
    {
        public Column Column { get; private set; }
        Expression<Func<TDTO, Dictionary<Column, object>, object>> ActionOnRow { get; set; }
        public Expression<Func<TDTO, object>> PropertySelector { get; private set; }
        public string PropertyName { get; protected set; } = string.Empty;
        public bool IsList { get; private set; }

        private QueryMapperInfo()
        {

        }
        internal QueryMapperInfo(Column column, Expression<Func<TDTO, object>> propSelector, string propertyName, bool isList)
        {
            Column = column;
            PropertySelector = propSelector;
            PropertyName = propertyName;
            IsList = isList;
        }
    }
    public class QueryMapper<TDTO> where TDTO : class
    {
        List<QueryMapperInfo<TDTO>> Mappers { get; set; } = new();

        public QueryMapperInfo<TDTO>? GetMapperFor(Column column)
            => Mappers.SingleOrDefault(x => x.Column.ColumnName == column.ColumnName);

        public QueryMapperInfo<TDTO>? GetMapperFor(string columnName)
            => Mappers.SingleOrDefault(x => x.Column.ColumnName == columnName);

        public static QueryMapper<TDTO> For(Expression<Func<TDTO, object>> propSelector, Column column)
        {
            var name = propSelector.GetMemberName();
            var lst = new List<QueryMapperInfo<TDTO>>
            {
                new QueryMapperInfo<TDTO>(column, propSelector, name, typeof(TDTO).IsArrayOrList())
            };
            return new QueryMapper<TDTO> { Mappers = lst };
        }
        public static QueryMapper<TDTO> For(QueryMapper<TDTO> queryMapper, Expression<Func<TDTO, object>> propSelector, Column column)
        {
            var name = propSelector.GetMemberName();
            queryMapper.Mappers.Add(new QueryMapperInfo<TDTO>(column, propSelector, name, typeof(TDTO).IsArrayOrList()));
            return queryMapper;
        }
    }
}
