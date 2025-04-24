using Enyim.Caching;
using event_horizon_backend.Core.Cache.Interfaces;

namespace event_horizon_backend.Core.Cache.Providers;

public class MemcachedService : ICacheService
{
    private readonly IMemcachedClient _cache;
    
    public MemcachedService(IMemcachedClient cache)
    {
        _cache = cache;
    }
    
    public async Task<T?> GetAsync<T>(string key)
    {
        return (T?)await _cache.GetAsync<T>(key);
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        await _cache.SetAsync(key, value, expiration ?? TimeSpan.FromMinutes(30));
    }
    
    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
    
    public async Task<bool> ExistsAsync(string key)
    {
        return await _cache.GetAsync<object>(key) != null;
    }
}