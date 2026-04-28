namespace Portfolio.Application.Users.Requests;

/// <summary>
/// Request model for logging in a User
/// </summary>
public record LoginRequest(
	string Email,
	string Password
)
{
	public LoginRequest() : this(string.Empty, string.Empty)
	{
	}
}