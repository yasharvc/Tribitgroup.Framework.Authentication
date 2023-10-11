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
            where TDTO : class => QueryMapper<TDTO>.For(propSelector, column);
    }
}
