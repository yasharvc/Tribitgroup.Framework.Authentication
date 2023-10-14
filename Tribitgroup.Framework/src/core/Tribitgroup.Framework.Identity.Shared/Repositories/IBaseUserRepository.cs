using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Identity.Shared.Repositories
{
    public interface IBaseUserRepository<TUser> : ICUDRepository<TUser, Guid>, IQueryRepository<TUser, Guid>
        where TUser : BaseUser, IAggregateRoot
    {
    }
}