using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Portfolio.Application.Services;

namespace Portfolio.Infrastructure.Services;

public class TokenRevocationService(IMemoryCache cache, IConfiguration configuration) : ITokenRevocationService
{
	private static string CacheKey(Guid UserId) => $"revoked_user_{UserId}";

	public Task RevokeUserAsync(Guid UserId)
	{
		var expirationMinutes = configuration.GetValue<int>("Jwt:ExpirationInMinutes", 60);

		cache.Set(CacheKey(UserId), true, TimeSpan.FromMinutes(expirationMinutes));

		return Task.CompletedTask;
	}

	public bool IsRevoked(Guid UserId)
	{
		return cache.TryGetValue(CacheKey(UserId), out _);
	}
}