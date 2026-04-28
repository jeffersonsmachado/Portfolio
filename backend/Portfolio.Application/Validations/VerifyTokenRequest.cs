namespace Portfolio.Application.Validations;

/// <summary>
/// Request model for verifying a user's email token
/// </summary>
public record VerifyTokenRequest(
	string Email,
	string Token
)
{
	public VerifyTokenRequest() : this(string.Empty, string.Empty)
	{
	}
}