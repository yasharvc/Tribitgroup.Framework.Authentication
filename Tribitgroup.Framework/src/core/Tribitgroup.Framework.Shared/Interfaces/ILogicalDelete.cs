namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface ILogicalDelete
    {
        bool Deleted { get; }
        Task DeleteAsync();
        Task Undelete();
    }
}