namespace Portfolio.Application.Users.Requests;

/// <summary>
/// Request model for updating a User
/// </summary>
public record UpdateUserRequest(
	string Username,
	string Email
)
{
	public UpdateUserRequest() : this(string.Empty, string.Empty)
	{
	}
}