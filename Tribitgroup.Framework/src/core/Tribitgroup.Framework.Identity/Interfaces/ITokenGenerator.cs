using Tribitgroup.Framework.Identity.Models;

namespace Tribitgroup.Framework.Identity.Interfaces
{
    public interface ITokenGenerator
    {
        Task<TokenInfo> GetTokenAsync(UserInfo userInfo);
    }
}
