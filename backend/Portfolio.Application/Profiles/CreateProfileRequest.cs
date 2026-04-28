namespace Portfolio.Application.Profiles;

/// <summary>
/// Request model for creating a new profile
/// </summary>
public class CreateProfileRequest
{
	public string Name { get; set; } = string.Empty;
	public Guid UserId { get; set; }
}
