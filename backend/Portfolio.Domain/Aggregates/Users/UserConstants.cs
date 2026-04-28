namespace Portfolio.Domain.Aggregates.Users;

public static class UserConstants
{
	/// <summary>
	/// Minimum length for a user's password
	/// </summary>
	public const int MinPasswordLength = 8;

	/// <summary>
	/// Maximum length for a user's password
	/// </summary>
	public const int MaxPasswordLength = 64;

	/// <summary>
	/// Minimum length for a user's name
	/// </summary>
	public const int MinNameLength = 3;

	/// <summary>
	/// Maximum length for a user's name
	/// </summary>
	public const int MaxNameLength = 20;

	/// <summary>
	/// UserName pattern: allows alphanumeric characters, underscores and hyphens, but no spaces.
	/// This is a simple pattern and can be adjusted based on specific requirements.
	/// </summary>
	public const string NamePattern = @"^[a-zA-Z0-9_-]+$";
}