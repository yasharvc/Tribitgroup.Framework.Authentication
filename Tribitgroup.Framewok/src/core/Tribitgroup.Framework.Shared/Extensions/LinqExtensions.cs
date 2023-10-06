﻿using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Tribitgroup.Framework.Shared.Enums;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Shared.Extensions
{

    public static class LinqExtensions
    {
        public static IQueryable<T> Pagination<T>(this IQueryable<T> query, int pageIndex, int pageSize, out int totalCount)
        {
            totalCount = query.Count();
            return query.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string orderByExpression)
        {
            if (string.IsNullOrEmpty(orderByExpression))
                return query;

            string propertyName, orderByMethod;
            string[] strs = orderByExpression.Split(' ');
            propertyName = strs[0];

            if (strs.Length == 1)
                orderByMethod = "OrderBy";
            else
                orderByMethod = strs[1].Equals("DESC", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";

            ParameterExpression pe = Expression.Parameter(query.ElementType);
            MemberExpression me = Expression.Property(pe, propertyName);

            MethodCallExpression orderByCall = Expression.Call(typeof(Queryable), orderByMethod, new Type[] { query.ElementType, me.Type }, query.Expression
                , Expression.Quote(Expression.Lambda(me, pe)));

            return query.Provider.CreateQuery(orderByCall) as IQueryable<T> ?? throw new Exception();
        }

        public static IQueryable<T> WhereEqual<T>(this IQueryable<T> query, string propertyName, object value)
        {
            if (string.IsNullOrEmpty(propertyName))
                return query;

            return query.Where($"{propertyName} = {value}");
        }

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, string whereClause)
        {
            if (!condition || string.IsNullOrEmpty(whereClause))
                return query;

            return query.Where(whereClause);
        }

        public static IQueryable<T> WhereIf<T>(this IEnumerable<T> query, bool condition, string whereClause)
        {
            if (!condition || string.IsNullOrEmpty(whereClause))
                return query.AsQueryable();

            return query.AsQueryable().Where(whereClause);
        }



        public static IQueryable<T> Where<T>(this IQueryable<T> query, Condition condition)
        {
            var config = new ParsingConfig
            {
                IsCaseSensitive = false
            };
            if (condition.Operator == ConditionOperatorEnum.Contains)
            {
                return query.Where(config, $"{condition.PropertyName}.Contains(@0)", condition.Values.First());
            }
            else if (condition.Operator == ConditionOperatorEnum.In)
            {
                return query.Where($"{condition.PropertyName} in @0", condition.Values);
            }
            else if (condition.Operator == ConditionOperatorEnum.Between)
            {
                return query.Where($"{condition.PropertyName} >= @0 AndAlso {condition.PropertyName} <= @1", condition.Values.First(), condition.Values.Skip(1).Take(1));
            }
            else if (condition.Operator == ConditionOperatorEnum.Equal)
            {
                return query.Where($"{condition.PropertyName} == @0", condition.Values.First());
            }
            else if (condition.Operator == ConditionOperatorEnum.BiggerThan)
            {
                return query.Where($"{condition.PropertyName} > @0", condition.Values.First());
            }
            else if (condition.Operator == ConditionOperatorEnum.BiggerOrEqual)
            {
                return query.Where($"{condition.PropertyName} >= @0", condition.Values.First());
            }
            else if (condition.Operator == ConditionOperatorEnum.SmallerOrEqual)
            {
                return query.Where($"{condition.PropertyName} <= @0", condition.Values.First());
            }
            else if (condition.Operator == ConditionOperatorEnum.SmallerThan)
            {
                return query.Where($"{condition.PropertyName} < @0", condition.Values.First());
            }
            else if (condition.Operator == ConditionOperatorEnum.NotEqual)
            {
                return query.Where($"{condition.PropertyName} != @0", condition.Values.First());
            }
            return query;
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> query, params Condition[] conditions)
        {
            foreach (var item in conditions)
            {
                query = query.Where(item);
            }
            return query;
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> query, IEnumerable<Condition> conditions) => query.Where(conditions.ToArray());

        public static IQueryable<T> Sort<T>(this IQueryable<T> query, IEnumerable<Sort>? sorts)
        {
            if (sorts == null)
                return query;

            var sortStr = "";

            foreach (var sort in sorts)
                sortStr += $"{(sortStr.Length > 0 ? "," : "")}{sort}";
            if(sortStr.Length > 0)
                return query.OrderBy(sortStr);
            return query;
        }
    }
}
