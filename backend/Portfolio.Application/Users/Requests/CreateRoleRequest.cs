namespace Portfolio.Application.Users.Requests;

/// <summary>
/// Request model for creating a new Role
/// </summary>
/// <param name="Name">Name of the Role</param>
public record CreateRoleRequest(
	string Name
)
{
	public CreateRoleRequest() : this(string.Empty)
	{
	}
}