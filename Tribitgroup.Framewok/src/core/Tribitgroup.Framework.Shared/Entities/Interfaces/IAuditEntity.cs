namespace Tribitgroup.Framework.Shared.Entities.Interfaces
{
    public interface IAuditEntity
    {
        DateTime CreatedAt { get; }
        Guid CreatedBy { get; }
    }
}