namespace Portfolio.Domain.Aggregates.Permissions;

public static class PermissionConstants
{
	/// <summary>
	/// Minimum length for a permission's name
	/// </summary>
	public const int MinNameLength = 3;

	/// <summary>
	/// Maximum length for a permission's name
	/// </summary>
	public const int MaxNameLength = 20;


	/// <summary>
	/// PermissionName pattern: allows alphanumeric characters, underscores and hyphens, but no spaces.
	/// This is a simple pattern and can be adjusted based on specific requirements.
	/// </summary>
	public const string NamePattern = @"^[a-zA-Z0-9_-]+$";
}