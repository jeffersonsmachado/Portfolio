using Portfolio.Application.Validations;
using Portfolio.Application.Users.Dtos;
using Portfolio.Domain.Shared;
using Portfolio.Application.Users.Requests;

namespace Portfolio.Application.Users;

/// <summary>
/// Service interface for Authentication-related operations
/// </summary>
/// 
/// <remarks>
/// This service handles user authentication, including login and email verification.
/// It is separate from IUserService to maintain a clear separation of concerns between user management and authentication logic.
/// </remarks>
public interface IAuthService
{
	Task<Result<UserDto>> VerifyToken(VerifyTokenRequest request, CancellationToken cancellationToken = default);
	Task<Result<LoginDto>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
	Task<Result<string>> ResendVerificationTokenAsync(ResendVerificationTokenRequest request, CancellationToken cancellationToken = default);
	Task<Result<UserDto>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}