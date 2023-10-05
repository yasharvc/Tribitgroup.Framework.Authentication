using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;
using Tribitgroup.Framewok.Shared.Entities.Interfaces;
using Tribitgroup.Framewok.Shared.Extensions;

namespace Tribitgroup.Framewok.Identity.Models
{
    public class ApplicationUser : IdentityUser<Guid>, IEntity<Guid>
    {
        public virtual ICollection<UserRefreshToken> RefreshTokens { get; set; } = new Collection<UserRefreshToken>();
        public virtual ICollection<UserTenant<ApplicationUser>> Tenants { get; set; } = new Collection<UserTenant<ApplicationUser>>();
        public virtual ICollection<UserPermission<ApplicationUser, ApplicationPermission>> Permissions { get; set; } = new Collection<UserPermission<ApplicationUser, ApplicationPermission>>();

        public ApplicationUser()
        {
            Id = BasicTypesExtensions.GetSequentialGuid();
        }
    }
}