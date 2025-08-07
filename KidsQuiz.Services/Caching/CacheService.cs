using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace KidsQuiz.Services.Caching
{
    public interface ICacheService
    {
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
        void Remove(string key);
        void Clear();
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _defaultOptions;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
            _defaultOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (_cache.TryGetValue(key, out T cachedValue))
            {
                _logger.LogDebug("Cache hit for key: {CacheKey}", key);
                return cachedValue;
            }

            _logger.LogDebug("Cache miss for key: {CacheKey}, executing factory", key);
            var value = await factory();
            var options = expiration.HasValue 
                ? new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(expiration.Value)
                    .SetAbsoluteExpiration(expiration.Value * 2)
                : _defaultOptions;

            _cache.Set(key, value, options);
            _logger.LogDebug("Cached value for key: {CacheKey} with expiration: {Expiration}", key, expiration ?? TimeSpan.FromMinutes(30));
            return value;
        }

        public void Remove(string key)
        {
            _logger.LogDebug("Removing cache entry for key: {CacheKey}", key);
            _cache.Remove(key);
        }

        public void Clear()
        {
            _logger.LogInformation("Clearing all cache entries");
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }
    }
} 