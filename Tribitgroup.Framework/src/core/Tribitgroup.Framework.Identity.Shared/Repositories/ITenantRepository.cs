using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Identity.Shared.Repositories
{
    public interface ITenantRepository : ICUDRepository<Role, Guid>, IQueryRepository<Tenant, Guid>
    {
    }
}