namespace Portfolio.Application.Profiles;

/// <summary>
/// Data Transfer Object for Profile entity
/// </summary>
public class ProfileDto
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string UserName { get; set; } = string.Empty;
}
