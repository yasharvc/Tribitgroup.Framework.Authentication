using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Identity.Shared.Repositories
{
    public interface IPolicyRepository : ICUDRepository<Role, Guid>, IQueryRepository<Policy, Guid>
    {
    }
}