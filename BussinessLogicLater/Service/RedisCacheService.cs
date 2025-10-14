using BussinessLogicLater.IService;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BussinessLogicLater.Service
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        public RedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
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

    }
}
