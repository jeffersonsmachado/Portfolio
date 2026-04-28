using Portfolio.Application.Users.Requests;
using Portfolio.Application.Validations;
using Portfolio.Domain.Shared;

namespace Portfolio.Application.Users;

/// <summary>
/// Service interface for User-related operations
/// </summary>
public interface IUserService
{
	public Task<Result<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
	public Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	public Task<Result<UserDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
	public Task<Result<IEnumerable<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default);
	public Task<Result<UserDto>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
