using Microsoft.AspNetCore.Identity;
using Tribitgroup.Framewok.Shared.Extensions;

namespace Tribitgroup.Framewok.Identity.Shared.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            Id = BasicTypesExtensions.GetSequentialGuid();
        }
    }
}