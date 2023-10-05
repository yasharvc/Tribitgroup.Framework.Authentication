using Tribitgroup.Framewok.Shared.Entities.Interfaces;

namespace Tribitgroup.Framewok.Shared.Entities
{
    public class AuditEntity : Entity, IEntity<Guid>, IAuditEntity
    {
        public DateTime CreatedAt { get; }

        public Guid CreatedBy { get; }

        public AuditEntity(Guid createdBy)
        {
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy;
        }

        public AuditEntity() : this(Guid.Empty) { }
    }
}