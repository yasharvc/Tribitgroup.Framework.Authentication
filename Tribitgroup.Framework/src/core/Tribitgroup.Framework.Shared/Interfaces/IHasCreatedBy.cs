namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface IHasCreatedBy
    {
        Guid CreatedBy { get; }
        Task SetCreatedByAsync(Guid userId);
    }
}