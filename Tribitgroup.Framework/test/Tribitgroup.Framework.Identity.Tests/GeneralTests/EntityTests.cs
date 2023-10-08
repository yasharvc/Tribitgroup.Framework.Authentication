using Microsoft.EntityFrameworkCore;
using Shouldly;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Tests.GeneralTests
{
    public class MyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Productasdasds { get; set; }
        // Add other DbSet properties here

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure your database connection here
        }
    }

    public class User : Entity
    {
        public string Name { get; set; }
    }

    public class Product : Entity
    {
        public string Name { get; set; }
    }

    public class EntityTests
    {
        [Fact]
        public void Test()
        {
            var name = new Product().GetTableName(new MyDbContext());

            name.ShouldBe(nameof(MyDbContext.Productasdasds));
        }
    }
}
