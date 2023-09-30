using Microsoft.AspNetCore.Identity;
using Tribitgroup.Framewok.Shared.Extensions;

namespace Tribitgroup.Framewok.Identity.Shared.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public ApplicationUser()
        {
            Id = BasicTypesExtensions.GetSequentialGuid();
        }
    }
}