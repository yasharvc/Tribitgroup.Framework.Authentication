namespace Tribitgroup.Framewok.Shared.Entities.Interfaces
{
    public interface IAggrigateRoot<T> : IEntity<T> { }
    public interface IAggrigateRoot : IEntity<Guid> { }
}