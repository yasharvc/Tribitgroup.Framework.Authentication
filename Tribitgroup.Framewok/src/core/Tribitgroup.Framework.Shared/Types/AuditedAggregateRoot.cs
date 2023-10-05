using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Shared.Types
{
    public abstract class AuditedAggregateRoot<T> : Entity<T>, IAggregateRoot where T : notnull
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        public AuditedAggregateRoot()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
    public abstract class AuditedAggregateRoot : AuditedAggregateRoot<Guid>, IAggregateRoot
    {
    }
}