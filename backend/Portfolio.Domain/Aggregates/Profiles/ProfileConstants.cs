namespace Portfolio.Domain.Aggregates.Profiles;

/// <summary>
/// Contains validation rules and constraints for Profile aggregate
/// </summary>

public static class ProfileConstants
{
	/// <summary>
	/// Minimum length for a profile's name
	/// </summary>
	public const int MinNameLength = 3;

	/// <summary>
	/// Maximum length for a profile's name
	/// </summary>
	public const int MaxNameLength = 20;

	/// <summary>
	/// Profile name pattern: allows alphanumeric characters, underscores, and hyphens
	/// </summary>
	public const string NamePattern = @"^[a-zA-Z0-9_-]+$";
}