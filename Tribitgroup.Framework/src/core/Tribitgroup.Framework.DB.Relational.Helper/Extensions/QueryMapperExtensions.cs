using System.Linq.Expressions;
using Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder;

namespace Tribitgroup.Framework.DB.Relational.Helper.Extensions
{
    public static class QueryMapperExtensions
    {
        public static QueryMapper<TDTO> For<TDTO>(
            this QueryMapper<TDTO> queryMapper,
            Expression<Func<TDTO, object>> propSelector,
            Column column)
            where TDTO : class => QueryMapper<TDTO>.For(queryMapper, propSelector, column);

        public static QueryMapper<TDTO> For<TDTO>(
            this Column column,
            Expression<Func<TDTO, object>> propSelector)
            where TDTO : class
                => QueryMapper<TDTO>.For(propSelector, column);

        public static QueryMapper<TDTO> ForObjectMember<TDTO, TOBJECT>(
            this QueryMapper<TDTO> queryMapper,
            Expression<Func<TDTO, TOBJECT>> propertSelector,
                Expression<Func<TOBJECT, object>> innerPropertySelector,
                Column column
            )
            where TDTO : class
            where TOBJECT : class 
                => QueryMapper<TDTO>.ForObjectMember(queryMapper, propertSelector, innerPropertySelector, column);

        public static QueryMapper<TDTO> ForList<TDTO, TInside>(
            this QueryMapper<TDTO> queryMapper,
            Expression<Func<TDTO, IEnumerable<TInside>>> listSelector,
            Expression<Func<TInside, object>> propertySelector,
            Column column)
            where TDTO : class
            where TInside : class
            => QueryMapper<TDTO>.ForListMember(queryMapper, listSelector, propertySelector, column);
    }
}
