using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Aggregates.Profiles;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Aggregates.Permissions;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Infrastructure.Persistence;

public class PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : DbContext(options)
{
	// Table
	public DbSet<Profile> Profiles => Set<Profile>();
	public DbSet<User> Users => Set<User>();
	public DbSet<Role> Roles => Set<Role>();
	public DbSet<Permission> Permissions => Set<Permission>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Configure Profile entity
		modelBuilder.Entity<Profile>(entity =>
		{
			entity.ToTable("profiles");

			entity.HasKey(e => e.Id);
			entity.Property(e => e.Id)
				.ValueGeneratedNever();

			entity.Property(e => e.Name)
				.IsRequired()
				.HasMaxLength(ProfileConstants.MaxNameLength)
				.HasColumnType($"character varying({ProfileConstants.MaxNameLength})");

			entity.Property(e => e.Bio)
				.HasMaxLength(500)
				.HasDefaultValue(string.Empty);

			entity.Property(e => e.BioTitle)
				.HasMaxLength(100)
				.HasDefaultValue(string.Empty);

			entity.Property(e => e.AvatarUrl)
				.HasMaxLength(2048)
				.HasDefaultValue(string.Empty);

			entity.HasOne(e => e.User)
				.WithMany(u => u.Profiles)
				.HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade);
		});

		// Configure User entity
		modelBuilder.Entity<User>(entity =>
		{
			entity.ToTable("users");

			entity.HasKey(e => e.Id);
			entity.Property(e => e.Id)
				.ValueGeneratedNever();

			entity.Property(e => e.Name)
				.HasConversion(
					v => v.Value,
					v => UserName.Create(v)
				)
				.IsRequired()
				.HasMaxLength(UserConstants.MaxNameLength)
				.HasColumnType($"character varying({UserConstants.MaxNameLength})");

			entity.Property(e => e.Email)
				.HasConversion(
					v => v.Value,
					v => Email.Create(v))
				.IsRequired()
				.HasMaxLength(254)
				.HasColumnType("character varying(254)");

			entity.Property(e => e.Password)
				.HasConversion(
					v => v.Value,
					v => PasswordHash.Create(v))
				.IsRequired()
				.HasMaxLength(UserConstants.MaxPasswordLength)
				.HasColumnType($"character varying({UserConstants.MaxPasswordLength})");

			entity.Property(e => e.IsEmailVerified)
				.IsRequired()
				.HasDefaultValue(false);

			entity.Property(e => e.CreatedAt)
				.HasColumnType("timestamp with time zone")
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.IsRequired();

			entity.Property(e => e.VerificationToken)
				.HasMaxLength(100)
				.HasColumnType("character varying(100)")
				.IsRequired(false);

			entity.Property(e => e.VerificationTokenExpiresAt)
				.HasColumnType("timestamp with time zone")
				.IsRequired(false);

			entity.HasIndex(u => u.Email)
				.IsUnique();

			entity.HasMany(u => u.Profiles)
				.WithOne(p => p.User)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasMany(u => u.Roles)
				.WithMany(r => r.Users)
				.UsingEntity<Dictionary<string, object>>(
					"user_roles",
					j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
					j => j.HasOne<User>().WithMany().HasForeignKey("UserId")
				);
		});

		// Configure Role entity
		modelBuilder.Entity<Role>(entity =>
		{
			entity.ToTable("roles");

			entity.HasKey(e => e.Id);
			entity.Property(e => e.Id)
				.ValueGeneratedNever();

			entity.Property(e => e.Name)
				.IsRequired()
				.HasMaxLength(RoleConstants.MaxNameLength)
				.HasColumnType($"character varying({RoleConstants.MaxNameLength})");

			entity.HasMany(r => r.Permissions)
				.WithMany(p => p.Roles)
				.UsingEntity<Dictionary<string, object>>(
					"role_permissions",
					j => j.HasOne<Permission>().WithMany().HasForeignKey("PermissionId"),
					j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId")
				);
		});

		// Configure Permission entity
		modelBuilder.Entity<Permission>(entity =>
		{
			entity.ToTable("permissions");

			entity.HasKey(e => e.Id);
			entity.Property(e => e.Id)
				.ValueGeneratedNever();

			entity.Property(e => e.Name)
				.IsRequired()
				.HasMaxLength(PermissionConstants.MaxNameLength)
				.HasColumnType($"character varying({PermissionConstants.MaxNameLength})");
		});
	}
}