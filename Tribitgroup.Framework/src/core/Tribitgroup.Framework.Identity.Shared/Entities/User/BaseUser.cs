using Tribitgroup.Framework.Shared.Interfaces;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Shared.Entities.User
{
    public abstract class BaseUser : AuditedAggregateRoot, IHasActiveStatus, ILogicalDelete
    {
        public bool IsActive { get; protected set; } = false;
        public bool IsLocked { get; protected set; } = false;
        public bool Deleted { get; protected set; } = false;
        public DateTime? LastLoginDateTime { get; protected set; }
        public virtual ICollection<UserToken> Tokens { get; set; } = new List<UserToken>();
        public virtual ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();
        public virtual ICollection<UserTenant> Tenants { get; set; } = new List<UserTenant>();
        public virtual ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
        public virtual ICollection<UserPermission> Permissions { get; set; } = new List<UserPermission>();
        public virtual ICollection<UserPolicy> Policies { get; set; } = new List<UserPolicy>();
        public virtual ICollection<UserDevice> Devices { get; set; } = new List<UserDevice>();
        public virtual ICollection<UserActivityLog> ActivityLogs { get; set; } = new List<UserActivityLog>();

    }
}