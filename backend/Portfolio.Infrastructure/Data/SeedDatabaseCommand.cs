using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Portfolio.Domain.Services;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Data;

public static class SeedDatabaseCommand
{
	public static async Task SeedDatabaseAsync(this IHost host)
	{
		using var scope = host.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
		var logger = scope.ServiceProvider.GetRequiredService<ILogger<PortfolioDbContext>>();
		var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

		await PortfolioDbContextSeed.SeedAsync(context, logger, passwordHasher);
	}
}
