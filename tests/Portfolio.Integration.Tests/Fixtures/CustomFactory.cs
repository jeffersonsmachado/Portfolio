using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Application.Services;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Integration.Tests.Fixtures;

public class CustomFactory : WebApplicationFactory<Program>
{
	private const string TestSecretKey = "test-secret-key-for-integration-tests-only";
	private const string TestIssuer = "PortfolioApi";
	private const string TestAudience = "PortfolioClients";

	private readonly SqliteConnection _connection = new("DataSource=:memory:");

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment("Testing");

		builder.ConfigureAppConfiguration((_, config) =>
		{
			config.AddInMemoryCollection(new Dictionary<string, string?>
			{
				["Jwt:SecretKey"] = TestSecretKey,
				["Jwt:Issuer"] = TestIssuer,
				["Jwt:Audience"] = TestAudience,
			});
		});

		// Open the connection — it must stay open for the in-memory DB lifetime
		_connection.Open();

		builder.ConfigureServices(services =>
		{
			// Remove ALL EF Core related registrations (Npgsql provider included)
			var descriptorsToRemove = services
				.Where(d => d.ServiceType == typeof(DbContextOptions<PortfolioDbContext>)
						  || d.ServiceType == typeof(PortfolioDbContext)
						  || d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true)
				.ToList();

			foreach (var descriptor in descriptorsToRemove)
			{
				services.Remove(descriptor);
			}

			// Add SQLite in-memory — relational, so ToTable/HasForeignKey work
			services.AddDbContext<PortfolioDbContext>(options =>
				options.UseSqlite(_connection));

			// Replace SMTP service with no-op stub to avoid real network calls in tests
			var emailDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));
			if (emailDescriptor != null)
				services.Remove(emailDescriptor);

			services.AddScoped<IEmailService, NoOpEmailService>();
		});
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		_connection.Dispose();
	}

	public HttpClient CreateAuthenticatedClient(Guid? userId = null)
	{
		var client = CreateClient();
		var token = GenerateTestJwt(userId ?? Guid.NewGuid());
		client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
		return client;
	}

	public static string GenerateTestJwt(Guid userId)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSecretKey));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
			new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
		};
		var token = new JwtSecurityToken(
			issuer: TestIssuer,
			audience: TestAudience,
			claims: claims,
			expires: DateTime.UtcNow.AddHours(1),
			signingCredentials: credentials
		);
		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	private sealed class NoOpEmailService : IEmailService
	{
		public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
			=> Task.CompletedTask;
	}

}