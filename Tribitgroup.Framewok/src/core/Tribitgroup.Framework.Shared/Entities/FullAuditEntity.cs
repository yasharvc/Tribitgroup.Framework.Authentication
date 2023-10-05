using Tribitgroup.Framework.Shared.Entities.Interfaces;

namespace Tribitgroup.Framework.Shared.Entities
{
    public class FullAuditEntity : AuditEntity, IAuditEntity, IFullAuditEntity
    {
        public DateTime? LastUpdatedAt { get; }

        public Guid? LastUpdatedBy { get; }
    }
}