using System.Collections;
using System.Linq.Dynamic.Core;
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


        public static void SetMemberValue<T>(this T entity, Expression<Func<T, object>> expression, object? value)
        {
            var propName = entity.GetMemberName(expression);
            entity.SetMemberValue(propName, value);
        }

        public static void SetMemberValue<T>(this T entity, string propName, object? value)
        {
            var prop = entity?.GetType().GetProperty(propName);
            prop?.SetValue(entity, entity.ChangeTo(prop.PropertyType, value));
        }

        public static void AddValueToListMember<T>(this T entity, Expression<Func<T, object>> expression, params object[] values)
        {
            var propName = entity.GetMemberName(expression);

            var prop = entity?.GetType().GetProperty(propName) ?? throw new Exception("Property is not accessable");
            if(prop.GetValue(entity) == null)
            {
                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(prop.PropertyType.GenericTypeArguments[0]) ?? throw new Exception();

                var instance = (IList)(Activator.CreateInstance(constructedListType) ?? throw new InvalidCastException());

                foreach (var item in values)
                    instance.Add(item);

                prop.SetValue(entity, instance);
            }
            else
            {
                var lst = (IList)(prop.GetValue(entity) ?? throw new InvalidCastException());
                foreach (var item in values)
                    lst.Add(item);
            }
        }

        public static string GetMemberName<T>(this T instance, Expression<Func<T, object>> expression)
        {
            return GetMemberName(expression.Body);
        }
        public static List<string> GetMemberNames<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            var memberNames = new List<string>();
            foreach (var cExpression in expressions)
            {
                memberNames.Add(GetMemberName(cExpression.Body));
            }
            return memberNames;
        }

        public static string GetMemberName<T>(this Expression<Func<T, object>> expression) => GetMemberName(expression.Body);
        private static string GetMemberName(Expression expression)
        {
            string expressionCannotBeNullMessage = "The expression cannot be null.";
            string invalidExpressionMessage = "Invalid expression.";
            if (expression == null)
            {
                throw new ArgumentException(expressionCannotBeNullMessage);
            }
            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpression = (MemberExpression)expression;
                return memberExpression.Member.Name;
            }
            if (expression is MethodCallExpression)
            {
                // Reference type method
                var methodCallExpression = (MethodCallExpression)expression;
                return methodCallExpression.Method.Name;
            }
            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                return GetMemberName(unaryExpression);
            }
            throw new ArgumentException(invalidExpressionMessage);
        }
        private static string GetMemberName(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression)
            {
                var methodExpression = (MethodCallExpression)unaryExpression.Operand;
                return methodExpression.Method.Name;
            }
            return ((MemberExpression)unaryExpression.Operand).Member.Name;
        }
    }
}
