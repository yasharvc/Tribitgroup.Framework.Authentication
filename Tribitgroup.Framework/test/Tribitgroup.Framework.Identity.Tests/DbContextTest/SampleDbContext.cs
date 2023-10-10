using Dapper.Contrib.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Tests.DbContextTest
{
    public class User : AggregateRoot
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }

    //[Table("Orders")]
    public class Order : AggregateRoot
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    //[Table("OrderDetails")]
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Order>()
                .HasMany(m => m.OrderDetails)
                .WithOne(m => m.Order);

            builder.Entity<Order>()
                .HasOne(m => m.User);
        }

    }
}
