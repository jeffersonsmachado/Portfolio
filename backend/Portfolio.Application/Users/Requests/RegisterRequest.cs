namespace Portfolio.Application.Users.Requests;

/// <summary>
/// Request model for logging in a User
/// </summary>
public record RegisterRequest(
	string Username,
	string Email,
	string Password,
	string ConfirmPassword
)
{
	public RegisterRequest() : this(string.Empty, string.Empty, string.Empty, string.Empty)
	{
	}
}