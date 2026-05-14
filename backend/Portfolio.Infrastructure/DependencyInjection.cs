using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Profiles;
using Portfolio.Application.Users;
using Portfolio.Domain.Aggregates.Profiles;
using Portfolio.Domain.Aggregates.Roles;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Services;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Infrastructure.Persistence.Repositories;
using Portfolio.Infrastructure.Services;
using Portfolio.Application.Services;
using MassTransit;
using Portfolio.Infrastructure.Messaging;
using Portfolio.Application.Events;

namespace Portfolio.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration,
		bool IsDevelopment = true)
	{

		#region Database Context

		if (IsDevelopment)
		{

			string connectionString = configuration.GetConnectionString("PostgresConnectionString")
				?? configuration.GetConnectionString("DefaultConnection")
				?? throw new InvalidOperationException("No valid connection string found.");

			services.AddDbContext<PortfolioDbContext>(options =>
				// options.UseSqlite(connectionString));
				options.UseNpgsql(connectionString, npgsqlOptions =>
				{
					npgsqlOptions
						.MigrationsAssembly(typeof(PortfolioDbContext).Assembly.FullName)
						.EnableRetryOnFailure
						(
							maxRetryCount: 5,
							maxRetryDelay: TimeSpan.FromSeconds(10),
							errorCodesToAdd: null
						);
				}
				)
			);

		}
		else
		{
			// services.AddScoped<Profiles.IProfileRepository, Profiles.DB_CONTEXT/>();
		}

		#endregion

		#region Repositories

		services.AddScoped<IProfileRepository, ProfileRepository>();
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IRoleRepository, RoleRepository>();

		#endregion

		#region Services

		// DOMAIN
		services.AddScoped<IPasswordHasher, PasswordHasher>();

		// APPLICATION
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<IProfileService, ProfileService>();
		services.AddScoped<IRoleService, RoleService>();
		services.AddScoped<IUserRoleService, UserRoleService>();

		// INFRASTRUCTURE
		services.AddScoped<SmtpEmailService>();
		services.AddScoped<IEmailService, RabbitMqEmailPublisher>();
		services.AddScoped<IJwtProvider, JwtProvider>();
		services.AddScoped<IEventBus, MassTransitEventBus>();

		services.AddSingleton<ITokenRevocationService, TokenRevocationService>();

		services.AddMassTransit(x =>
		{
			x.AddConsumer<EmailMessageConsumer>();
			x.AddConsumer<UserRegisteredConsumer>();
			x.AddConsumer<UserDeletedConsumer>();
			x.AddConsumer<UserRolesUpdatedConsumer>();
			x.AddConsumer<AuditConsumer>();
			x.UsingRabbitMq((ctx, cfg) =>
			{
				cfg.Host(configuration["RabbitMq:Host"]!, "/", h =>
				{
					h.Username(configuration["RabbitMq:Username"]!);
					h.Password(configuration["RabbitMq:Password"]!);
				});

				cfg.ConfigureEndpoints(ctx);
			});

		});

		services.AddMemoryCache();

		#endregion

		return services;
	}
}
