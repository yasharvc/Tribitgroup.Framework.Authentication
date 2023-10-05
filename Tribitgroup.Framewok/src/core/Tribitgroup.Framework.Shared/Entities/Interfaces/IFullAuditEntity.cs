namespace Tribitgroup.Framework.Shared.Entities.Interfaces
{
    public interface IFullAuditEntity : IAuditEntity
    {
        DateTime? LastUpdatedAt { get; }
        Guid? LastUpdatedBy { get; }
    }
}
