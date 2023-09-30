using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framewok.Identity.Shared.Models;

namespace Tribitgroup.Framewok.Identity
{
    public class StandardDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public DbSet<UserRefreshToken> RefreshTokens => Set<UserRefreshToken>();
        public DbSet<UserPermission<ApplicationUser, ApplicationPermission>> UserPermissions => Set<UserPermission<ApplicationUser, ApplicationPermission>>();
        public StandardDbContext(DbContextOptions<StandardDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder
                .Entity<ApplicationUser>()
                .HasMany(m => m.RefreshTokens);
            builder
                .Entity<ApplicationUser>()
                .HasMany(m => m.Permissions);
        }
    }
}