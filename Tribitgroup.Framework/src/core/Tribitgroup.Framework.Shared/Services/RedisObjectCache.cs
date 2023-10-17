using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tribitgroup.Framework.Shared.Extensions;
using Tribitgroup.Framework.Shared.Interfaces;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Shared.Services
{
    public class RedisObjectCache<TValue> : ICache<string, TValue> where TValue : class
    {
        public TimeSpan? AutoClearTime { get; private set; } = TimeSpan.Zero;
        public ulong MaxSizeInBytes { get; private set; } = 1024 * 2;
        protected RedisSetting RedisSetting { get; private set; }
        protected IDatabase Cache { get; private set; }

        public RedisObjectCache(RedisSetting redisSetting, ulong maxSizeInBytes, TimeSpan? autoClearTime = null)
        {
            if (autoClearTime.HasValue && autoClearTime.Value < TimeSpan.Zero)
                throw new InvalidDataException(nameof(autoClearTime));

            AutoClearTime = autoClearTime ?? TimeSpan.Zero;
            RedisSetting = redisSetting;
            MaxSizeInBytes = maxSizeInBytes;
            Cache = RedisSetting.Connect();
        }

        public async Task<TValue?> GetAsync(string key)
        {
            var json = await Cache.StringGetAsync(key);
            return json.HasValue ? json.ToString().FromJson<TValue>() : null;
        }

        public async Task<IEnumerable<TValue>> GetAllAsync()
        {
            var keyValues = new Dictionary<string, TValue>();
            var keys = await GetKeysAsync();

            foreach (var key in keys)
                keyValues[key] = await GetAsync(key);

            return keyValues.Values;
        }

        public async Task AddOrUpdateAsync(string key, TValue value, TimeSpan? expiration = null) 
            => await Cache.StringSetAsync(key, value.ToJson(), expiration ?? AutoClearTime);

        public Task RemoveAsync(string key) => Cache.StringGetDeleteAsync(key);

        public Task<bool> ContainsKeyAsync(string key) 
            => Cache.KeyExistsAsync(key);

        public async Task ClearAsync()
        {
            var keys = await GetKeysAsync();
            foreach(var key in keys)
                await RemoveAsync(key);
        }

        private Task<IEnumerable<string>> GetKeysAsync()
        {
            long cursor = 0;
            var keys = new List<string>();

            do
            {
                var scanResult = Cache.Execute("SCAN", cursor.ToString());
                var innerResult = (RedisResult[])scanResult;

                cursor = long.Parse((string)innerResult[0]);

                List<string> resultLines = ((string[])innerResult[1]).ToList();
                keys.AddRange(resultLines);

            } while (cursor != 0);
            return Task.FromResult(keys.AsEnumerable());
        }
    }
}
