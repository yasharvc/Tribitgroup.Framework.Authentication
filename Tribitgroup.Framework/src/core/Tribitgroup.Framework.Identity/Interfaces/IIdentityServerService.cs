using Tribitgroup.Framework.Identity.Shared.DTO;
using Tribitgroup.Framework.Identity.Models;

namespace Tribitgroup.Framework.Identity.Interfaces
{
    public interface IIdentityServerService<
        TUser,
        TRole,
        TPermission> 
        where TUser : ApplicationUser
        where TRole: ApplicationRole
        where TPermission : ApplicationPermission
    {
        Task<Guid> RegisterAsync(RegisterWithUsernameEmailPasswordInputDTO input, CancellationToken cancellationToken = default);
        Task AddRoleToUserAsync(Guid userId, params string[] roleNames);
        Task<TRole> CreateRoleAsync(string roleName);
        Task<IEnumerable<TPermission>> CreatePermissionAsync(params TPermission[] permissions);
        Task<IEnumerable<TPermission>> GetAllPermissionAsync();
        Task<IEnumerable<TRole>> GetAllRolesAsync();
    }
}