using Shouldly;
using Tribitgroup.Framework.Shared.Enums;
using Tribitgroup.Framework.Shared.Extensions;
using Tribitgroup.Framework.DB.Relational.Helper.Extensions;
using Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder;

namespace Tribitgroup.Framework.Identity.Tests.DbContextTest.DB
{
    public class DbTests
    {
        [Fact]
        public void GetTableName_Should_Return_Books()
        {
            var book = new Book();
            book.GetTableName().ShouldBe("Books");
        }
        [Fact]
        public void GetAsColumns_Should_List_All_Inhereted_Props()
        {
            var lst = new BookTemp().GetAsColumns();

            lst.ShouldContain(m => m.ColumnName == nameof(Book.Name));
            lst.ShouldContain(m => m.ColumnName == nameof(Book.Id));
            lst.ShouldContain(m => m.ColumnName == nameof(BookTemp.IsTemp));
            lst.ShouldNotContain(m => m.ColumnName == "Age");
            lst.ShouldContain(m => m.TableName == new BookTemp().GetTableName());
        }
        [Fact]
        public void GetAsColumns_Should_List_All_Public_Props()
        {
            var lst = new Book().GetAsColumns();

            lst.ShouldContain(m => m.ColumnName == nameof(Book.Name));
            lst.ShouldContain(m => m.ColumnName == nameof(Book.Id));
            lst.ShouldNotContain(m => m.ColumnName == "Age");
            lst.ShouldContain(m => m.TableName == new Book().GetTableName());
        }

        [Fact]
        public void GetAsColumns_ByType_Should_List_All_Public_Props()
        {
            var lst = typeof(Book).GetAsColumns();

            lst.ShouldContain(m => m.ColumnName == nameof(Book.Name));
            lst.ShouldContain(m => m.ColumnName == nameof(Book.Id));
            lst.ShouldNotContain(m => m.ColumnName == "Age");
            lst.ShouldContain(m => m.TableName == new Book().GetTableName());
        }
        [Fact]
        public void GetAsColumns_ByGenericType_Should_List_All_Public_Props()
        {
            var lst = QueryExtensions.GetAsColumns<Book>();

            lst.ShouldContain(m => m.ColumnName == nameof(Book.Name));
            lst.ShouldContain(m => m.ColumnName == nameof(Book.Id));
            lst.ShouldNotContain(m => m.ColumnName == "Age");
            lst.ShouldContain(m => m.TableName == new Book().GetTableName());
        }
        [Fact]
        public void TestQuery()
        {
            var res1 = Query.From<Borrow,Guid>() ?? throw new Exception();
            res1.Select(() => {
                var b = new Borrow();
                var cols = new List<Column>
                {
                    new Column
                    {
                        ColumnName = nameof(Borrow.Id),
                        TableName = b.GetTableName()
                    },
                    new Column
                    {
                        ColumnName = nameof(b.StudentId),
                        TableName = b.GetTableName()
                    },
                    new Column
                    {
                        ColumnName = nameof(b.BookId),
                        TableName = b.GetTableName()
                    }
                };
                return cols;
            });

            var res = Query.From(res1) ?? throw new Exception();

            res.Select(() => {
                var b = new Borrow();
                var bo = new Book();
                var cols = new List<Column>
                {
                    new Column
                    {
                        ColumnName = nameof(bo.Name),
                        TableName = bo.GetTableName(),
                        Alias = "BookName"
                    },
                    new Column
                    {
                        ColumnName = nameof(b.StudentId),
                        TableName = b.GetTableName(),
                        Alias = "StudentID"
                    }
                };
                return cols;
            });

            res.InnerJoin<Book, Guid>(() =>
            {
                var bo = new Borrow();
                var b = new Book();
                return new List<RelationColumn> {
                    new RelationColumn {
                        FromColumn = nameof(bo.BookId),
                        ToColumn = nameof(b.Id)
                    },
                    new RelationColumn {
                        FromColumn = nameof(bo.BookId),
                        IsRelationalCondition = false,
                        Operator = ConditionOperatorEnum.NotEqual,
                        ToColumn = "100"
                    },
                };
            });

            res.Where(() =>
            {
                var res = ConditionMaker.Create($"{new Book().GetTableName()}.{nameof(Book.Name)}", ConditionOperatorEnum.Equal, "C#");
                return res;
            });

            var str = res.ToString();
            str.ShouldNotBeEmpty();
        }
    }
}
