using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Shared.Services
{
    public class InMemoryEntityCache<TEntity> : InMemoryCache<Guid, TEntity>, IEntityCache<TEntity>, ICache<Guid, TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public InMemoryEntityCache(long maxSizeInBytes, TimeSpan? autoClearTime = null) : base(maxSizeInBytes, autoClearTime) { }
        public Task AddOrUpdateAsync(TEntity value, TimeSpan? expiration = null) => AddOrUpdateAsync(value.Id, value, expiration);
        public async Task AddOrUpdateAsync(params TEntity[] values) => await AddOrUpdateAsync(null, values);
        public async Task AddOrUpdateAsync(TimeSpan? expiration, params TEntity[] values)
        {
            foreach (var value in values)
                await AddOrUpdateAsync(value.Id, value, expiration);
        }
    }
}
