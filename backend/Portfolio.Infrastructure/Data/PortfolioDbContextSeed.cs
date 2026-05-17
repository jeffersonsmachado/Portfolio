using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Portfolio.Domain.Aggregates.Profiles;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.ValueObjects;
using Portfolio.Infrastructure.Persistence;
using Portfolio.Domain.Services;
using Portfolio.Domain.Shared;

namespace Portfolio.Infrastructure.Data;

public static class PortfolioDbContextSeed
{
	public static async Task SeedAsync(PortfolioDbContext context, ILogger<PortfolioDbContext> logger, IPasswordHasher passwordHasher)
	{
		if (await context.Users.AnyAsync())
		{
			logger.LogInformation("The database has been seeded");
			return;
		}

		var admin = User.Create("admin", "admin@mail.com", PasswordHash.CreateFromRaw("secret123", passwordHasher).Value!, string.Empty, true).Value!;
		var johnDoe = User.Create("John Doe", "john@mail.com", PasswordHash.CreateFromRaw("secret123", passwordHasher).Value!, string.Empty, true).Value!;
		var janeDoe = User.Create("Jane Doe", "jane@mail.com", PasswordHash.CreateFromRaw("secret123", passwordHasher).Value!, string.Empty, true).Value!;

		List<User> users =
		[
			admin,
			johnDoe,
			janeDoe
		];

		await context.Users.AddRangeAsync(users);
		await context.SaveChangesAsync();

		logger.LogInformation("Seeded users to the database");

		var profileAdminProfile = Profile.Create("Admin Profile", admin);
		var profileOneProfile = Profile.Create("ProfileOne", users[1]);
		var profileTwoProfile = Profile.Create("ProfileTwo", users[2]);

		List<Profile> profiles =
		[
			profileAdminProfile,
			profileOneProfile,
			profileTwoProfile
		];

		await context.Profiles.AddRangeAsync(profiles);
		await context.SaveChangesAsync();

		logger.LogInformation("Seeded profiles to the database");

		List<Permission> adminPermissions =
		[
			Permission.Create(Permissions.ProfileView),
			Permission.Create(Permissions.ProfileCreate),
			Permission.Create(Permissions.ProfileUpdate),
			Permission.Create(Permissions.ProfileDelete),

			Permission.Create(Permissions.UserView),
			Permission.Create(Permissions.UserCreate),
			Permission.Create(Permissions.UserUpdate),
			Permission.Create(Permissions.UserDelete),

			Permission.Create(Permissions.RolesView),
			Permission.Create(Permissions.RolesCreate),
			Permission.Create(Permissions.RolesUpdate),
			Permission.Create(Permissions.RolesDelete),
		];

		await context.Permissions.AddRangeAsync(adminPermissions);
		await context.SaveChangesAsync();

		logger.LogInformation("Seeded permissions to the database");

		// Reuse the already-tracked objects — never create new instances for existing rows
		var permissionLookup = adminPermissions.ToDictionary(p => p.Name);
		string[] userPermissionNames = [Permissions.ProfileView, Permissions.ProfileCreate, Permissions.ProfileUpdate];
		List<Permission> userPermissions = [.. userPermissionNames.Select(n => permissionLookup[n])];

		Role userRole = Role.Create("User", userPermissions, [johnDoe, janeDoe]);
		Role adminRole = Role.Create("Admin", adminPermissions, [admin]);

		List<Role> roles =
		[
			adminRole,
			userRole
		];

		await context.Roles.AddRangeAsync(roles);
		await context.SaveChangesAsync();

		logger.LogInformation("Seeded roles to the database");
	}
}