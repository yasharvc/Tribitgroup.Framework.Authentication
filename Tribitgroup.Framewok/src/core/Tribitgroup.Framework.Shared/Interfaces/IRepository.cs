namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface IRepository<T, U> : IQueryRepository<T, U>, ICUDRepository<T, U> where T : class, IEntity<U>, IAggregateRoot where U : notnull
    {
    }
}