using Microsoft.Extensions.Caching.Memory;
using System;

namespace LoginPageAPI.Services
{
    /// <summary>
    /// Caching service implementation using IMemoryCache.
    /// </summary>
    public class CachingService : ICachingService
    {
        private readonly IMemoryCache _cache;
        public CachingService(IMemoryCache cache)
        {
            _cache = cache;
        }
        public T? Get<T>(string key) => _cache.TryGetValue(key, out T value) ? value : default;
        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            if (expiration.HasValue)
                _cache.Set(key, value, expiration.Value);
            else
                _cache.Set(key, value);
        }
        public void Remove(string key) => _cache.Remove(key);
    }
}
