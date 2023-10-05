using Microsoft.AspNetCore.Identity;
using Tribitgroup.Framework.Shared.Extensions;

namespace Tribitgroup.Framework.Identity.Models
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