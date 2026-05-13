namespace Portfolio.Application.Services;

/// <summary>
/// Interface for Token Revocation Service
/// </summary>
public interface ITokenRevocationService
{
	Task RevokeUserAsync(Guid UserId);
	bool IsRevoked(Guid UserId);
}