using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framework.Identity.Shared.Consts;
using Tribitgroup.Framework.Identity.Models;

namespace Tribitgroup.Framework.Identity
{
    public class StandardDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IIdentityDbContext<ApplicationUser, ApplicationRole, ApplicationPermission>
    {
        public DbSet<UserRefreshToken> RefreshTokens => Set<UserRefreshToken>();
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<UserTenant<ApplicationUser>> UserTenants => Set<UserTenant<ApplicationUser>>();
        public DbSet<UserPermission<ApplicationUser, ApplicationPermission>> UserPermissions => Set<UserPermission<ApplicationUser, ApplicationPermission>>();

        public DbSet<ApplicationPermission> Permissions => Set<ApplicationPermission>();

        public DbSet<ApplicationUser> GetUserDbSet() => Set<ApplicationUser>();

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
            builder
                .Entity<ApplicationUser>()
                .HasMany(m => m.Tenants);

            builder.Entity<ApplicationRole>()
                .HasData(new ApplicationRole { Name = DefaultRoles.UserAdmin });
        }
    }
}