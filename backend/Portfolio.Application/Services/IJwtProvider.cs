using Portfolio.Domain.Aggregates.Users;

namespace Portfolio.Application.Services;

/// <summary>
/// Interface for JWT token generation and validation
/// </summary>
public interface IJwtProvider
{
	string Generate(User user);
}