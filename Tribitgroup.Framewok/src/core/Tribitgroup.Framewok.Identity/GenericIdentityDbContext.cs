using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framewok.Identity.Shared.Models;

namespace Tribitgroup.Framewok.Identity
{
    public abstract class GenericIdentityDbContext<TUser,TRole> : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<TUser,TRole, Guid> where TUser: ApplicationUser where TRole : ApplicationRole
    {
        public GenericIdentityDbContext(DbContextOptions<GenericIdentityDbContext<TUser, TRole>> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}