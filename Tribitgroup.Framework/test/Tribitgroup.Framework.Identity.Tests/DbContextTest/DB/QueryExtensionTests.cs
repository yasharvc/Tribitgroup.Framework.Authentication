using Shouldly;
using System.Linq.Expressions;
using System.Reflection;
using Tribitgroup.Framework.DB.Relational.Helper.Extensions;
using Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder;
using Tribitgroup.Framework.Shared.Enums;

namespace Tribitgroup.Framework.Identity.Tests.DbContextTest.DB
{
    public class QueryExtensionTests
    {
        [Fact]
        public void QueryExtensionTest()
        {
            var query = Query.From<Borrow>()
                .InnerJoin<Borrow, Book>(b => b.BookId)
                .InnerJoin<Borrow, Student>(b => b.StudentId)
                .InnerJoin<ISBN, Book>(b => b.BookId)
                .SelectColumns(() =>
                {
                    return new List<Column>
                    {
                        Column.From<Book>(m=>m.Name, "BookName"),
                        Column.From<Student>(m=>m.FirstName,"StudentName"),
                        Column.From<Borrow>(m=>m.StudentId),
                        Column.From<ISBN>(m=>m.Code,"ISBN")
                    };
                })
                .AddCondition((Student s) => s.Id, ConditionOperatorEnum.Equal, 2)
                .ToString();

            query.ShouldNotBeEmpty();
        }


        class MyBaseClass
        {
            public int BaseProperty { get; set; }
        }

        class MyDerivedClass : MyBaseClass
        {
            public int DerivedProperty { get; set; }
        }


        public PropertyInfo GetPropertyInfo<TSource, TProperty>(
    Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }   
    }
}
