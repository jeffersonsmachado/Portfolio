namespace Portfolio.Application.Validations;

/// <summary>
/// Request model for resending a user's email verification token
/// </summary>
public record ResendVerificationTokenRequest(
	string Email
)
{
	public ResendVerificationTokenRequest() : this(string.Empty)
	{
	}
}