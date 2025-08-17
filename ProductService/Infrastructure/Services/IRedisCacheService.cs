namespace ProductService.Infrastructure.Services;

public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key);
    
    Task SetAsync<T>(string key, T value, int expirationMinutes = 30);
    
    Task RemoveAsync(string key);
    
    Task RemovePatternAsync(string pattern);
}
