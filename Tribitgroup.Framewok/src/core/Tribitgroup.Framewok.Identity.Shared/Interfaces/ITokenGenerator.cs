using Tribitgroup.Framewok.Identity.Shared.Models;

namespace Tribitgroup.Framewok.Identity.Shared.Interfaces
{
    public interface ITokenGenerator
    {
        Task<TokenInfo> GetTokenAsync(UserInfo userInfo);
    }
}
