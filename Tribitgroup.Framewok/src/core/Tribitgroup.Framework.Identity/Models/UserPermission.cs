using Tribitgroup.Framework.Shared.Entities;

namespace Tribitgroup.Framework.Identity.Models
{
    public class UserPermission<TUser, TPermission> : Entity where TUser: ApplicationUser where TPermission : ApplicationPermission
    {
        public Guid ApplicationUserId { get; set; }
        public TUser ApplicationUser { get; set; }
        public Guid ApplicationPermissionId { get; set;}
        public TPermission ApplicationPermission { get; set; }

    }
}