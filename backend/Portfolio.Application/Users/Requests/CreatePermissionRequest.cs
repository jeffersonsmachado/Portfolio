namespace Portfolio.Application.Users.Requests;

/// <summary>
/// Request model for creating a new Permission
/// </summary>
/// <param name="Name">Name of the Permission</param>
public record CreatePermissionRequest(
	string Name
)
{
	public CreatePermissionRequest() : this(string.Empty)
	{
	}
}
