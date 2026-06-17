namespace Portfolio.Application.Users.Dtos;

/// <summary>
/// Data Transfer Object for Login entity
/// </summary>
public class LoginDto
{
	public string Token { get; set; } = string.Empty;

	/// <summary>
	/// Translated capability map keyed by resource (e.g. "profile", "user", "role").
	/// The frontend uses this directly — it never sees raw roles or permission strings.
	/// </summary>
	public Dictionary<string, ResourceCapabilities> Capabilities { get; set; } = [];
	public UserDto? User { get; set; }
}
