namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface IHasActiveStatus
    {
        bool IsActive { get; }
        Task Activate();
        Task Deactivate();
    }
}