using Tribitgroup.Framewok.Identity.Shared.DTO;
using Tribitgroup.Framewok.Identity.Shared.Models;

namespace Tribitgroup.Framewok.Identity.Shared.Interfaces
{
    public interface IIdentityServerService
    {
        Task<Guid> RegisterAsync(RegisterWithUsernameEmailPasswordInputDTO input, CancellationToken cancellationToken = default);
        Task AddRoleToUserAsync(Guid userId, params string[] roleNames);
        Task<Guid> AddRoleAsync(string roleName);
        Task<Guid> AddPermissionAsync<TPermission>(TPermission permission) where TPermission: ApplicationPermission;
    }
}