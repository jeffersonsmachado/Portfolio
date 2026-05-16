namespace Portfolio.Application.Users.Requests;

/// <summary>
/// Request model for setting role permissions
/// </summary>
public record SetRolePermissionRequest(
	IEnumerable<Guid> PermissionIds
)
{
	public SetRolePermissionRequest() : this([])
	{
	}
}