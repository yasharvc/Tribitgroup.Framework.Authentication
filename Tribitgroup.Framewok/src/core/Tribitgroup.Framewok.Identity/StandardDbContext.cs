using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framewok.Identity.Shared.Consts;
using Tribitgroup.Framewok.Identity.Shared.Models;

namespace Tribitgroup.Framewok.Identity
{
    public class StandardDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IIdentityDbContext<ApplicationUser, ApplicationRole, ApplicationPermission>
    {
        public DbSet<UserRefreshToken> RefreshTokens => Set<UserRefreshToken>();
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

            builder.Entity<ApplicationRole>()
                .HasData(new ApplicationRole { Name = DefaultRoles.UserAdmin });
        }
    }
}