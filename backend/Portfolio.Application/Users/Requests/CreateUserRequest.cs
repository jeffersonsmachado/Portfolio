namespace Portfolio.Application.Users.Requests;

/// <summary>
/// Request model for creating a new User
/// </summary>
public record CreateUserRequest(
	string Username,
	string Email,
	string Password,
	string ConfirmPassword
)
{
	public CreateUserRequest() : this(string.Empty, string.Empty, string.Empty, string.Empty)
	{
	}
}