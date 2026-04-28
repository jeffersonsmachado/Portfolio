using Microsoft.AspNetCore.Authorization;
using Portfolio.Api.Authorization;

namespace Portfolio.Api.Extensions;

public static class AuthorizationExtensions
{
	public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
	{
		services.AddAuthorization();
		services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

		return services;
	}
}
