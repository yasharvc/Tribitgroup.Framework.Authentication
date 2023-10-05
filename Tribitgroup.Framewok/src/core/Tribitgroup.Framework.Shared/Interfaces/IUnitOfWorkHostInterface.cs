namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface IUnitOfWorkHostInterface : IUnitOfWorkHostEvents
    {
        object DbContext { get; }
    }
}
