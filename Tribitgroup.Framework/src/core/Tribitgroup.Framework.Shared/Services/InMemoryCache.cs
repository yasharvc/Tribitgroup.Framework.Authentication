using Microsoft.Extensions.Caching.Memory;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Shared.Services
{
    public class InMemoryCache<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull
    {
        public TimeSpan? AutoClearTime { get; private set; } = TimeSpan.Zero;
        public ulong MaxSizeInBytes { get; private set; } = 1024;
        protected MemoryCache Cache { get; private set; }

        public InMemoryCache(long maxSizeInBytes, TimeSpan? autoClearTime = null)
        {
            if (autoClearTime.HasValue && autoClearTime.Value < TimeSpan.Zero)
                throw new InvalidDataException(nameof(autoClearTime));

            AutoClearTime = autoClearTime ?? TimeSpan.Zero;
            MaxSizeInBytes = (ulong)maxSizeInBytes;
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = MaxSizeInBytes < 1024 ? 1024 : (long)MaxSizeInBytes,
            });
        }

        public Task<TValue?> GetAsync(TKey key) => Task.FromResult((TValue?)Cache.Get(key));

        public Task<IEnumerable<TValue>> GetAllAsync()
        {
            var cachedItems = new Dictionary<TKey,TValue>();

            var cacheEntriesCollectionDefinition = typeof(MemoryCache)
                .GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(Cache) as dynamic ?? throw new InvalidCastException();

            foreach (var cacheItem in cacheEntriesCollectionDefinition)
            {
                var item = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem);
                cachedItems[item.Key] = item.Value;
            }

            var res = cachedItems.Values.ToArray().AsEnumerable();
            return Task.FromResult(res);
        }

        public Task AddOrUpdateAsync(TKey key, TValue value, TimeSpan? expiration = null)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions();
            
            if (expiration.HasValue)
                cacheEntryOptions.SetAbsoluteExpiration(expiration.Value);
            else if (AutoClearTime.HasValue && AutoClearTime.Value.TotalMilliseconds > 0)
                cacheEntryOptions.SetAbsoluteExpiration(AutoClearTime.Value);

            Cache.Set(key, value, cacheEntryOptions.SetSize(1));
            return Task.CompletedTask;
        }

        public Task RemoveAsync(TKey key)
        {
            Cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task<bool> ContainsKeyAsync(TKey key) => Task.FromResult(Cache.Get(key) != null);

        public Task ClearAsync()
        {
            Cache.Clear();
            return Task.CompletedTask;
        }
    }
}
