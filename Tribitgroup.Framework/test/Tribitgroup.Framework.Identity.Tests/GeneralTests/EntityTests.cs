using Dapper;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Tribitgroup.Framework.Dapper;
using Tribitgroup.Framework.DB.Relational.Helper.SelectBuilder;
using Tribitgroup.Framework.Identity.Tests.DbContextTest;
using Tribitgroup.Framework.Shared.Types;
using Tribitgroup.Framework.Test.Helper;
using Tribitgroup.Framework.DB.Relational.Helper.Extensions;
using Tribitgroup.Framework.Identity.Tests.DbContextTest.DB;
using Tribitgroup.Framework.Shared.Extensions;

namespace Tribitgroup.Framework.Identity.Tests.GeneralTests
{
    public class MyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Productasdasds { get; set; }
        public DbSet<DbContextTest.Order> Orders => Set<DbContextTest.Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

        public MyDbContext()
        {
            
        }

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DbContextTest.Order>()
                .HasMany(m => m.OrderDetails)
                .WithOne(m => m.Order);

            builder.Entity<DbContextTest.Order>()
                .HasOne(m => m.User);
        }
    }

    public class User : Entity
    {
        public string Name { get; set; }
    }

    public class Product : AggregateRoot
    {
        public string Name { get; set; }
    }

    public class EntityTests : BaseTestClass<MyDbContext>
    {
        public EntityTests() : base("test")
        {
        }

        class Test
        {
            public string Name { get; set; } = string.Empty;
            public List<string> List { get; set; }
            public Book Book { get; set; }
            public virtual ICollection<Book> Books { get; set; }
        }

        [Fact]
        public void GetMemberName_Should_return_name_of_selected_propeerty()
        {
            var testObject = new Test();

            testObject.SetMemberValue(x => x.Name, "Yashar");
            testObject.AddValueToListMember(x => x.List, "1", "2");
            testObject.AddValueToListMember(x => x.List, "4", "3");
            testObject.SetMemberValue(x => x.Book, new Book { Name = "Book 1" });
            testObject.SetMemberValue(x => x.Books, new List<Book> { 
                new Book { Name = "Book 2" },
                new Book { Name = "Book 4" }
            });

            testObject.Name.ShouldBe("Yashar");
            testObject.List.ShouldNotBeEmpty();
            testObject.Book.ShouldNotBeNull();
        }

        [Fact]
        public void GetTableName_With_DbContext_Should_return_Property_Name_Of_Product_DbSet()
        {
            var name = new Product().GetTableName(new MyDbContext());

            name.ShouldBe(nameof(MyDbContext.Productasdasds));
        }

        [Fact]
        public async Task Test1()
        {
            var p1 = new Product { Name = "P1" };
            var p2 = new Product { Name = "P2" };
            var repo = new DapperCUDRepository<Product>(new DapperCUDConnectionProvider<Product>(GetDbContext()));

            await repo.InsertManyAsync(new List<Product> { p1, p2 });

            var res = await GetDbContext().Productasdasds.ToListAsync();

            res.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task JoinQueryTest()
        {
            var user = new DbContextTest.User { Username = "yashar", Password = "123", DateOfBirth = DateTime.Now };
            var order = new DbContextTest.Order
            {
                UserId = user.Id,
                User = user,
                OrderDetails = new List<OrderDetail>(),
            };

            var detail = new List<OrderDetail>{
                    new OrderDetail
                    {
                        Count = 5,
                        ItemName = "item 1",
                        Order = order,
                        OrderId = order.Id,
                        Price = 1000
                    },
                    new OrderDetail
                    {
                        Count = 10,
                        ItemName = "item 2",
                        Order = order,
                        OrderId = order.Id,
                        Price = 250
                    }
                };

            order.OrderDetails = detail;

            var ctx = GetDbContext();
            var conn = ctx.Database.GetDbConnection();

            await ctx.AddAsync(order);
            await ctx.SaveChangesAsync();

            var first = await ctx.Orders.FirstOrDefaultAsync();

            var query = @"SELECT Orders.*, OrderDetails.* FROM Orders INNER JOIN OrderDetails ON OrderDetails.OrderId = Orders.Id";

            var lst = await conn.QueryAsync(query);
            lst.ShouldNotBeEmpty();

            var col1 = Column.From<OrderDetail>(m => m.Count);
            var priceCol = Column.From<OrderDetail>(m => m.Price);

            var qry = Query.From<DbContextTest.Order>(GetDbContext())
                .InnerJoin<OrderDetail, DbContextTest.Order>(b=>b.OrderId)
                .SelectColumns(() =>
                {
                    return new List<Column>
                    {
                        Column.From<DbContextTest.Order>(m=>m.Id, "Order_Id"),
                        col1,
                        priceCol,
                        Column.From<OrderDetail>(m=>m.ItemName)
                    };
                });

            var ttt = await qry.RunAsync(conn, 
                QueryMapper<TestDTO>
                .For(dto => dto.Count, col1)
                .For(dto => dto.Price, priceCol)
                );

            var res2 = await conn.QueryAsync(qry.ToString());

            Assert.NotNull(first);
            first.OrderDetails.ShouldNotBeEmpty();
        }
    }
    class TestDTO : Entity
    {
        public Guid OrderId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
