using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Models
{
    public class UserTenant<TUser> : Entity where TUser : ApplicationUser
    {
        public Guid ApplicationUserId { get; set; }
        public TUser ApplicationUser { get; set; }
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }
}