namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface IUnitOfWorkHostEvents
    {
        event EventHandler Committed;
        event EventHandler RollBacked;
    }
}
