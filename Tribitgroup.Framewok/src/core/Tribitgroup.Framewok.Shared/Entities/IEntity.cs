namespace Tribitgroup.Framewok.Shared.Entities
{
    public interface IEntity<T>
    {
        T Id { get; }
    }
}