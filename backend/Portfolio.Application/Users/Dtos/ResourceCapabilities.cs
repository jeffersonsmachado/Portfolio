namespace Portfolio.Application.Users.Dtos;

/// <summary>
/// Represents what actions the authenticated user can perform on a given resource.
/// Used by the frontend to show/hide controls without knowing about roles or raw permission strings.
/// </summary>
public record ResourceCapabilities
{
	public bool CanCreate { get; init; }
	public bool CanRead { get; init; }
	public bool CanUpdate { get; init; }
	public bool CanDelete { get; init; }
}
