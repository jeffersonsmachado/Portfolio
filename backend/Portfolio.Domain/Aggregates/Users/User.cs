using Portfolio.Domain.Aggregates.Profiles;
using Portfolio.Domain.Services;
using Portfolio.Domain.Shared;
using Portfolio.Domain.ValueObjects;

namespace Portfolio.Domain.Aggregates.Users;

public class User
{
	#region Properties

	public Guid Id { get; private set; }
	public UserName Name { get; private set; }
	public Email Email { get; private set; }
	public PasswordHash Password { get; private set; } = null!;
	public bool IsEmailVerified { get; private set; } = false;
	public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
	public string? VerificationToken { get; private set; } = null;
	public DateTime? VerificationTokenExpiresAt { get; private set; } = null;
	public virtual ICollection<Role> Roles { get; private set; } = [];

	#endregion

	#region Navigation Properties

	public virtual ICollection<Profile> Profiles { get; private set; } = [];

	#endregion

	#region EF Core Constructor

	private User()
	{
		Name = UserName.Create(string.Empty);
		Email = Email.Create(string.Empty);
		Password = PasswordHash.Create(string.Empty);
	}

	private User(UserName name, Email email, PasswordHash password)
	{
		Id = Guid.NewGuid();
		Name = name;
		Email = email;
		Password = password;
	}

	// Factory method for creating a new user
	public static Result<User> Create(string name, string email, PasswordHash password, string verificationToken, bool isEmailVerified = false)
	{
		return Result<User>.Success(new User(UserName.Create(name), Email.Create(email), password)
		{
			Id = Guid.NewGuid(),
			CreatedAt = DateTime.UtcNow,
			IsEmailVerified = isEmailVerified,
			VerificationToken = verificationToken,
			VerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24)
		});
	}

	// Factory method for recreating from storage (or mock data)
	public static Result<User> Recreate(
		Guid id,
		string name,
		string email,
		PasswordHash password,
		DateTime createdAt,
		bool isEmailVerified,
		string? verificationToken = null,
		DateTime? verificationTokenExpiresAt = null)
	{
		if (id == Guid.Empty)
			return Result<User>.Failure(new Error("INVALID_USER_ID", "Id cannot be empty."));

		return Result<User>.Success(new User(UserName.Create(name), Email.Create(email), password)
		{
			Id = id,
			CreatedAt = createdAt,
			IsEmailVerified = isEmailVerified,
			VerificationToken = verificationToken,
			VerificationTokenExpiresAt = verificationTokenExpiresAt
		});
	}

	#endregion

	#region Methods

	public bool VerifyToken(string token)
	{
		if (IsEmailVerified)
		{
			return true; // Already verified
		}

		if (VerificationToken == null || VerificationTokenExpiresAt == null)
		{
			return false; // No token set
		}

		if (DateTime.UtcNow > VerificationTokenExpiresAt.Value)
		{
			return false; // Token expired
		}

		if (VerificationToken != token)
		{
			return false; // Invalid token
		}

		IsEmailVerified = true;
		VerificationToken = null;
		VerificationTokenExpiresAt = null;

		return true;
	}

	public void VerifyEmail(bool isEmailVerified)
	{
		IsEmailVerified = isEmailVerified;
	}

	public bool VerifyPassword(string password, IPasswordHasher passwordHasher)
	{
		return passwordHasher.Verify(password, Password.Value);
	}

	public void UpdateVerificationToken(string token)
	{
		VerificationToken = token;
		VerificationTokenExpiresAt = DateTime.UtcNow.AddHours(24);
	}

	public void AssignRole(Role role)
	{
		if (!Roles.Contains(role))
		{
			Roles.Add(role);
		}
	}

	public void RemoveRole(Role role)
	{
		Roles.Remove(role);
	}

	public void UpdateProfile(Profile profile)
	{
		if (!Profiles.Contains(profile))
		{
			Profiles.Add(profile);
		}
	}

	#endregion
}
