namespace Tribitgroup.Framewok.Shared.Entities.Interfaces
{
    public interface IAuditEntity
    {
        DateTime CreatedAt { get; }
        Guid CreatedBy { get; }
    }
}