using Tribitgroup.Framewok.Identity.Models;

namespace Tribitgroup.Framewok.Identity.Interfaces
{
    public interface IAuthenticator<T> where T : ApplicationUser
    {
        Task<UserInfo> AuthenticateWithUsernamePasswordAsync(string username, string password);
        Task<UserInfo> AuthenticateWithEmailPasswordAsync(string email, string password);
    }
}