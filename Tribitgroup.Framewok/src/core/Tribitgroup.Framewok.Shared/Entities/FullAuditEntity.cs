using Tribitgroup.Framewok.Shared.Entities.Interfaces;

namespace Tribitgroup.Framewok.Shared.Entities
{
    public class FullAuditEntity : AuditEntity, IAuditEntity, IFullAuditEntity
    {
        public DateTime? LastUpdatedAt { get; }

        public Guid? LastUpdatedBy { get; }
    }
}