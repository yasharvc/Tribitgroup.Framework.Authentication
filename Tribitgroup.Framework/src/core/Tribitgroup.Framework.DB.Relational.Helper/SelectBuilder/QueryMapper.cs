using System.Linq.Expressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using Tribitgroup.Framework.Shared.Extensions;

namespace Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder
{
    public class QueryMapperInfo<TDTO> where TDTO : class
    {
        public Column Column { get; private set; }
        public Expression<Func<TDTO, object>> PropertySelector { get; private set; }
        public string PropertyName { get; protected set; } = string.Empty;
        public bool IsListType { get; private set; }
        public bool IsObjectType { get; private set; }
        public bool IsBasicType { get; private set; }
        Expression<Func<TDTO, object>> ListSelector { get; set; }

        private QueryMapperInfo()
        {

        }
        internal QueryMapperInfo(
            Column column,
            Expression<Func<TDTO, object>> propSelector,
            string propertyName,
            bool isListType,
            bool isObjectType,
            bool isBasicType)
        {
            Column = column;
            PropertySelector = propSelector;
            PropertyName = propertyName;
            IsListType = isListType;
            IsObjectType = isObjectType;
            IsBasicType = isBasicType;
        }
    }
    public class QueryMapper<TDTO> where TDTO : class
    {
        List<QueryMapperInfo<TDTO>> Mappers { get; set; } = new();

        public QueryMapperInfo<TDTO>? GetMapperFor(Column column)
            => Mappers.SingleOrDefault(x => x.Column.ColumnName == column.ColumnName);

        public QueryMapperInfo<TDTO>? GetMapperFor(string columnName)
            => Mappers.SingleOrDefault(x => x.Column.ColumnName == columnName || x.Column.Alias == columnName);

        public static QueryMapper<TDTO> For(Expression<Func<TDTO, object>> propSelector, Column column)
        {
            var name = propSelector.GetMemberName();
            var type = typeof(TDTO).GetProperty(name).PropertyType;

            var lst = new List<QueryMapperInfo<TDTO>>
            {
                new QueryMapperInfo<TDTO>(
                    column,
                    propSelector,
                    name,
                    type.IsArrayOrList(),
                    !type.IsArrayOrList() && !type.IsBasicType(),
                    type.IsBasicType())
            };
            return new QueryMapper<TDTO> { Mappers = lst };
        }
        public static QueryMapper<TDTO> For(QueryMapper<TDTO> queryMapper, Expression<Func<TDTO, object>> propSelector, Column column)
        {
            var name = propSelector.GetMemberName();
            var type = typeof(TDTO).GetProperty(name).PropertyType;

            queryMapper.Mappers.Add(new QueryMapperInfo<TDTO>(
                column,
                propSelector,
                name,
                type.IsArrayOrList(),
                    !type.IsArrayOrList() && !type.IsBasicType(),
                    type.IsBasicType()));
            return queryMapper;
        }

        public static QueryMapper<TDTO> ForObjectMember<TOBJECT>(
            Expression<Func<TDTO, TOBJECT>> propertSelector,
            Expression<Func<TOBJECT, object>> innerPropertySelector,
            Column column)
            where TOBJECT : class
        {
            var propName = ((MemberExpression)propertSelector.Body).Member.Name;
            propName += $".{innerPropertySelector.GetMemberName()}";

            Expression<Func<TDTO, object>> test = (dto) => dto;

            var lst = new List<QueryMapperInfo<TDTO>>
            {
                new QueryMapperInfo<TDTO>(
                    column,
                    test,
                    propName,
                    false,
                    true,
                    false)
            };
            return new QueryMapper<TDTO> { Mappers = lst };
        }

        public static QueryMapper<TDTO> ForObjectMember<TOBJECT>(
                QueryMapper<TDTO> queryMapper,
                Expression<Func<TDTO, TOBJECT>> propertSelector,
                Expression<Func<TOBJECT, object>> innerPropertySelector,
                Column column)
                where TOBJECT : class
        {
            var propName = ((MemberExpression)propertSelector.Body).Member.Name;
            propName += $".{innerPropertySelector.GetMemberName()}";

            Expression<Func<TDTO, object>> test = (dto) => dto;

            queryMapper.Mappers.Add(new QueryMapperInfo<TDTO>(
                    column,
                    test,
                    propName,
                    false,
                    true,
                    false));

            return queryMapper;
        }
        
        public static QueryMapper<TDTO> ForListMember<TInside>(
            QueryMapper<TDTO> queryMapper,
            Expression<Func<TDTO, IEnumerable<TInside>>> listSelector,
            Expression<Func<TInside, object>> propertySelector,
            Column column) where TInside : class
        {
            var propName = ((MemberExpression)listSelector.Body).Member.Name;
            propName += $".{propertySelector.GetMemberName()}";
            Expression<Func<TDTO, object>> test = (dto) => dto;
            queryMapper.Mappers.Add(new QueryMapperInfo<TDTO>(column, test, propName, true, false, false));
            return queryMapper;
        }

        public static QueryMapper<TDTO> ForObjectMember(
            QueryMapper<TDTO> queryMapper, 
            Expression<Func<TDTO, object>> propSelector,
            Column column)
        {
            var name = propSelector.GetMemberName();
            var propType = typeof(TDTO).GetProperty(name)?.PropertyType;
            if (propType == null || propType.IsBasicType() || propType.IsArrayOrList())
                throw new NotSupportedException();

            //queryMapper.Mappers.Add(new QueryMapperInfo<TDTO>(column, propSelector, name, typeof(TDTO).IsArrayOrList()));
            //return queryMapper;
            throw new NotImplementedException();
        }

    }
}
