using Tribitgroup.Framewok.Shared.Entities;

namespace Tribitgroup.Framewok.Identity.Shared.Models
{
    public class UserPermission<TUser, TPermission> : Entity where TUser: ApplicationUser where TPermission : ApplicationPermission
    {
        public Guid ApplicationUserId { get; set; }
        public TUser ApplicationUser { get; set; }
        public Guid ApplicationPermissionId { get; set;}
        public TPermission ApplicationPermission { get; set; }

    }
}