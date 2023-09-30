using Microsoft.EntityFrameworkCore;
using Tribitgroup.Framewok.Identity.Shared.Models;

namespace Tribitgroup.Framewok.Identity
{
    public interface IIdentityDbContext<TUser, TRole, TPermission> 
        where TUser: ApplicationUser 
        where TRole : ApplicationRole
        where TPermission : ApplicationPermission
    {
        DbSet<UserRefreshToken> RefreshTokens { get; }
        DbSet<TPermission> Permissions { get; }
        DbSet<UserPermission<TUser, TPermission>> UserPermissions { get; }
        DbSet<TUser> GetUserDbSet();
    }
}
