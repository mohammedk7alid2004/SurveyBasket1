
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace SurveyBasket.BLL.Services;

public class CashService(IDistributedCache cache) : ICashService
{
    private readonly IDistributedCache _cache = cache;

    public async Task<T?> GetAsync<T>(string Key, CancellationToken cancellationToken = default) where T : class
    {
      var cashValue = await _cache.GetStringAsync(Key, cancellationToken);
        return cashValue is null ? null :
            JsonSerializer.Deserialize<T>(cashValue);
    }

    public async Task SetAsync<T>(string Key, T value, CancellationToken cancellationToken = default) where T : class
    {
        await _cache.SetStringAsync(Key, JsonSerializer.Serialize(value),cancellationToken);

    }
    public async Task RemoveAsync(string Key, CancellationToken cancellationToken = default)
    {
      await  _cache.RemoveAsync(Key, cancellationToken);
    }

    
}
