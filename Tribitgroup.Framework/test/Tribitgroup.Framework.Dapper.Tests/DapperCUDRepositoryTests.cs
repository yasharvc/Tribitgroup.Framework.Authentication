using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tribitgroup.Framework.Shared.Types;
using Tribitgroup.Framework.Test.Helper;

namespace Tribitgroup.Framework.Dapper.Tests
{
    public class User : AggregateRoot
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }

    public class Order : AggregateRoot
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    public class OrderDetail : Entity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Count { get; set; } = 1;
        public decimal Price { get; set; } = 0;
    }

    public class SampleDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();


        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Order>()
                .HasMany(m => m.OrderDetails)
                .WithOne(m => m.Order);

            builder.Entity<Order>()
                .HasOne(m => m.User);
        }

    }

    public class DapperCUDRepositoryTests : BaseTestClass<SampleDbContext>
    {
        DapperCUDConnectionProvider<User> DapperRepoProvider { get; }
        DapperCUDRepository<User> DapperUserCUDRepo { get; }
        public DapperCUDRepositoryTests() : base(nameof(SampleDbContext))
        {
            DapperRepoProvider = new DapperCUDConnectionProvider<User>(GetDbContext());
            DapperUserCUDRepo = new DapperCUDRepository<User>(DapperRepoProvider);
        }


        [Fact]
        public async Task InsertMany_Should_Add_Users()
        {
            var user1 = new User
            {
                DateOfBirth = DateTime.Now.AddYears(-20),
                Password = "password",
                Username = "User 1",
            };
            var user2 = new User
            {
                DateOfBirth = DateTime.Now.AddYears(-15),
                Password = "password",
                Username = "User 2",
            };

            await DapperUserCUDRepo.InsertManyAsync(new List<User> { user1, user2 });

            var lst = await GetDbContext().Users.ToListAsync();

            lst.Count.ShouldBe(2);
            Assert.NotNull(lst.SingleOrDefault(x => x.Id == user1.Id));
            Assert.NotNull(lst.SingleOrDefault(x => x.Id == user2.Id));
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_The_Data()
        {
            var users = await InsertTestUsersAsync();

            users.First().Username = "yashar";

            await DapperUserCUDRepo.UpdateOneAsync(users.First());

            var lst = await GetDbContext().Users.ToListAsync();

            lst.Count.ShouldBe(2);
            Assert.NotNull(lst.SingleOrDefault(x => x.Id == users.First().Id && x.Username == "yashar"));
        }


        private async Task<IEnumerable<User>> InsertTestUsersAsync()
        {
            var user1 = new User
            {
                DateOfBirth = DateTime.Now.AddYears(-20),
                Password = "password",
                Username = "User 1",
            };
            var user2 = new User
            {
                DateOfBirth = DateTime.Now.AddYears(-15),
                Password = "password",
                Username = "User 2",
            };

            var res = new List<User> { user1, user2 };

            await DapperUserCUDRepo.InsertManyAsync(res);
            return res;
        }
    }
}
