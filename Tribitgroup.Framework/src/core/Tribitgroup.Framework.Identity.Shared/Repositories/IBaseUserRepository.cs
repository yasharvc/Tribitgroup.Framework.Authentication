using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Identity.Shared.Entities.User;
using Tribitgroup.Framework.Identity.Shared.Enums;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Identity.Shared.Repositories
{
    public interface IBaseUserRepository<TUser> : ICUDRepository<TUser, Guid>, IQueryRepository<TUser, Guid>
        where TUser : BaseUser, IAggregateRoot
    {
        Task AddActivityLogAsync(TUser user, UserActivityEnum userActivity, string description, string? metadata = null);
        Task AddDeviceAsync(TUser user, UserDevice device);
        Task LockAsync(TUser user);
        Task UnlockAsync(TUser user);
        Task DeleteAsync(TUser user);
        Task ActivateAsync(TUser user);
        Task DeactivateAsync(TUser user);
        Task AddPermissionAsync(TUser user, params Permission[] permissions);
        Task RemovePermissionAsync(TUser user, params Permission[] permissions);
        Task AddRoleAsync(TUser user, params Role[] roles);
        Task RemoveRoleAsync(TUser user, params Role[] roles);
        Task AddTenantAsync(TUser user, params Tenant[] tenants);
        Task RemoveTenantAsync(TUser user, params Tenant[] tenants);
        Task AddPolicyAsync(TUser user, params Policy[] policies);
        Task AddTokenAsync(TUser user, Guid deviceId, string token, DateTime validUntil);
        Task DeactivateTokenAsync(Guid tokenId);
        Task UpdateLastLoginDateTimeAsynv(TUser user);
    }
}