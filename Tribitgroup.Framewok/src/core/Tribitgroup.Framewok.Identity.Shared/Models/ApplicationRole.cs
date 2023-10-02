using Microsoft.AspNetCore.Identity;
using Tribitgroup.Framewok.Shared.Extensions;

namespace Tribitgroup.Framewok.Identity.Shared.Models
{
    public class ApplicationRole : IdentityRole<Guid> {
        public ApplicationRole()
        {
            Id = BasicTypesExtensions.GetSequentialGuid();
        }
        public ApplicationRole(string roleName) : base(roleName)
        {
            Id = BasicTypesExtensions.GetSequentialGuid();
        }
    }
}