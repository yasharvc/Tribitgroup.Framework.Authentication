using Tribitgroup.Framewok.Identity.Models;

namespace Tribitgroup.Framewok.Identity.Interfaces
{
    public interface ITokenGenerator
    {
        Task<TokenInfo> GetTokenAsync(UserInfo userInfo);
    }
}
