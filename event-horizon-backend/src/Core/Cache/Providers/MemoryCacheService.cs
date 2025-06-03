using event_horizon_backend.Core.Cache.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace event_horizon_backend.Core.Cache.Providers;

// MemoryCacheService class that implements ICacheService interface for caching operations using IMemoryCache
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    
    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }
    
    public Task<T?> GetAsync<T>(string key)
    {
        return Task.FromResult(_cache.TryGetValue(key, out T value) ? value : default);
    }
    
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue)
            options.SetAbsoluteExpiration(expiration.Value);
            
        _cache.Set(key, value, options);
        return Task.CompletedTask;
    }
    
    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }
    
    public Task<bool> ExistsAsync(string key)
    {
        return Task.FromResult(_cache.TryGetValue(key, out _));
    }
}