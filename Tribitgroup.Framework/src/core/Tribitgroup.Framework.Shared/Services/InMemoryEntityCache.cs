using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Shared.Services
{
    public class InMemoryEntityCache<TEntity> : InMemoryCache<Guid, TEntity>, IEntityCache<TEntity>, ICache<Guid, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public InMemoryEntityCache(ulong maxSizeInBytes, TimeSpan? autoClearTime = null) : base(maxSizeInBytes, autoClearTime)
        {
        }
    }
}
