using Tribitgroup.Framework.Shared.Interfaces.Entity;

namespace Tribitgroup.Framework.Shared.Interfaces
{
    public interface IRepository<T, U> : IQueryRepository<T, U>, ICUDRepository<T, U> where T : class, IEntity<U> where U : notnull
    {
    }
}