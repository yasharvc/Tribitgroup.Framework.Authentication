using Tribitgroup.Framewok.Identity.Shared.Models;

namespace Tribitgroup.Framewok.Identity.Shared.Interfaces
{
    public interface IAuthenticator<T> where T : ApplicationUser
    {
        Task<UserInfo> AuthenticateWithUsernamePasswordAsync(string username, string password);
        Task<UserInfo> AuthenticateWithEmailPasswordAsync(string email, string password);
    }
}