using BussinessLogicLater.IService;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace BussinessLogicLater.Service
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        public RedisCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
        {
            var cachedData = await _distributedCache.GetStringAsync(key, cancellationToken);

            if (string.IsNullOrEmpty(cachedData))
                return default;

            var result = JsonSerializer.Deserialize<T>(cachedData);

            return result;
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken)
        {
            var jsonData = JsonSerializer.Serialize(value);

            await _distributedCache.SetStringAsync(key, jsonData, cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }

        public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken)
        {
            var server = GetServer();
            var db = _connectionMultiplexer.GetDatabase();
            var keys = server.Keys(pattern: $"{prefix}*").ToArray();

            foreach (var key in keys)
            {
                await db.KeyDeleteAsync(key);
            }
        }

        private IServer GetServer()
        {
            var endpoint = _connectionMultiplexer.GetEndPoints().First();
            return _connectionMultiplexer.GetServer(endpoint);
        }
    }
}
