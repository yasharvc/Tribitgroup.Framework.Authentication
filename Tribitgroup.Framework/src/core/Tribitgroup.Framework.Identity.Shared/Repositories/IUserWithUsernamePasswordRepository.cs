using Tribitgroup.Framework.Identity.Shared.Entities;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Identity.Shared.Repositories
{
    public interface IUserWithUsernamePasswordRepository : 
        IBaseUserRepository<UserWithUsernamePassword>,
        ICUDRepository<UserWithUsernamePassword, Guid>, IQueryRepository<UserWithUsernamePassword, Guid>
    {
        Task GetByUsernamePasswordAsync(string username, string password);
    }
}