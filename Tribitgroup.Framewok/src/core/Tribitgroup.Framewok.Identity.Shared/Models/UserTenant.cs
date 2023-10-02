using Tribitgroup.Framewok.Shared.Entities;

namespace Tribitgroup.Framewok.Identity.Shared.Models
{
    public class UserTenant<TUser> : Entity where TUser : ApplicationUser
    {
        public Guid ApplicationUserId { get; set; }
        public TUser ApplicationUser { get; set; }
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }
}