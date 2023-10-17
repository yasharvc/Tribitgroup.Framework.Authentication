using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Shared.Types
{
    public abstract class AuditedAggregateRoot<T> : Entity<T>, IHasCreatedAt, IHasCreatedBy, IAggregateRoot<T> where T : notnull
    {
        public Guid CreatedBy { get; protected set; }
        public DateTime CreatedOn {get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedAt { get; set; }

        public AuditedAggregateRoot()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public virtual Task SetCreatedByAsync(Guid userId)
        {
            CreatedBy = userId;
            return Task.CompletedTask;
        }
    }
    public abstract class AuditedAggregateRoot : AuditedAggregateRoot<Guid>, IAggregateRoot
    {
    }
}