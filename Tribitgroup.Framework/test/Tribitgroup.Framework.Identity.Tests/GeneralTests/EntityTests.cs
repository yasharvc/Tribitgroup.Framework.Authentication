using Microsoft.EntityFrameworkCore;
using Shouldly;
using Tribitgroup.Framework.Dapper;
using Tribitgroup.Framework.Shared.Types;
using Tribitgroup.Framework.Test.Helper;

namespace Tribitgroup.Framework.Identity.Tests.GeneralTests
{
    public class MyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Productasdasds { get; set; }
        // Add other DbSet properties here

        public MyDbContext()
        {
            
        }

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure your database connection here
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

        [Fact]
        public void GetTableName_With_DbContext_Should_return_Property_Name_Of_Product_DbSet()
        {
            var name = new Product().GetTableName(new MyDbContext());

            name.ShouldBe(nameof(MyDbContext.Productasdasds));
        }

        [Fact]
        public async Task Test()
        {
            var p1 = new Product { Name = "P1" };
            var p2 = new Product { Name = "P2" };
            var repo = new DapperCUDRepository<Product>(new DapperCUDConnectionProvider<Product>(GetDbContext()));

            await repo.InsertManyAsync(new List<Product> { p1, p2 });

            var res = await GetDbContext().Productasdasds.ToListAsync();

            res.ShouldNotBeEmpty();
        }
    }
}
