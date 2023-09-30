using Microsoft.AspNetCore.Identity;

namespace Tribitgroup.Framewok.Identity.Shared.Models
{
    public class ApplicationRole : IdentityRole<Guid> {
        public ApplicationRole()
        {
            
        }
        public ApplicationRole(string roleName) : base(roleName)
        {
            
        }
    }
}