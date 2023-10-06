using Tribitgroup.Framework.Identity.Models;

namespace Tribitgroup.Framework.Identity.Interfaces
{
    public interface IAuthenticator<T> where T : ApplicationUser
    {
        Task<UserInfo> AuthenticateWithUsernamePasswordAsync(string username, string password);
        Task<UserInfo> AuthenticateWithEmailPasswordAsync(string email, string password);
    }
}