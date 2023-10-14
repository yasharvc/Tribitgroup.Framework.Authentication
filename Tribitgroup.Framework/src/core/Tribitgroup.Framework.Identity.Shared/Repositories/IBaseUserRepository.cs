using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Identity.Shared.Entities.User;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Identity.Shared.Repositories
{
    public interface IBaseUserRepository<TUser> : ICUDRepository<TUser, Guid>, IQueryRepository<TUser, Guid>
        where TUser : BaseUser, IAggregateRoot
    {
        Task AddActivityLogAsync(TUser user, string description, string? metadata = null);
        Task AddDeviceAsync(TUser user, UserDevice device);
        Task LockAsync(TUser user);
        Task UnlockAsync(TUser user);
        Task DeleteAsync(TUser user);
        Task ActivateAsync(TUser user);
        Task DeactivateAsync(TUser user);
        Task AddPermissionAsync(TUser user, params Permission[] permissions);
    }
}