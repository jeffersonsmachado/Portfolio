using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Services;
using Portfolio.Domain.Shared;

namespace Portfolio.Domain.ValueObjects;

public record PasswordHash
{
	public string Value { get; init; }

	private PasswordHash(string value)
	{
		Value = value;
	}

	public static PasswordHash Create(string hash)
	{
		return new PasswordHash(hash);
	}

	public static Result<PasswordHash> CreateFromRaw(string rawPassword, IPasswordHasher passwordHasher)
	{
		var validation = ValidatePasswordRaw(rawPassword);
		if (validation.IsFailure)
			return Result<PasswordHash>.Failure(validation.Error!);

		var hashedPassword = passwordHasher.Hash(rawPassword);
		return Result<PasswordHash>.Success(new PasswordHash(hashedPassword));
	}

	public bool Verify(string rawPassword, IPasswordHasher passwordHasher)
	{
		return passwordHasher.Verify(rawPassword, Value);
	}

	private static Result<string> ValidatePasswordRaw(string rawPassword)
	{
		if (string.IsNullOrWhiteSpace(rawPassword))
			return Result<string>.Failure(new Error("INVALID_PASSWORD", "Password cannot be null or empty."));

		if (UserConstants.MinPasswordLength > 0 && rawPassword.Length < UserConstants.MinPasswordLength)
			return Result<string>.Failure(new Error("INVALID_PASSWORD", $"Password must be at least {UserConstants.MinPasswordLength} characters long."));

		return Result<string>.Success(rawPassword);
	}

}