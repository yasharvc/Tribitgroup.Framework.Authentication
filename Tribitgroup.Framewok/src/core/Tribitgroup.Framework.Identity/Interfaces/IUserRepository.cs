using Tribitgroup.Framework.Identity.Models;

namespace Tribitgroup.Framework.Identity.Interfaces
{
    public interface IUserRepository<T> where T : ApplicationUser
    {
        Task<T> CreateAsync(T user);
        Task<T> UpdateAsync(T user);
        Task DeleteAsync(T user);
        Task<T> GetByUsernamePasswordAsync(string username, string password);
        Task<T> GetByEmailPasswordAsync(string email, string password);
    }
}