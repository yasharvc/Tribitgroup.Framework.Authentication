using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Identity.Shared.Repositories
{
    public interface IUserActivityLogRepository : ICUDRepository<Role, Guid>, IQueryRepository<UserActivityLog, Guid>
    {
    }
}