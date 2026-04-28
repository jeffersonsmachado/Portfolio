using Portfolio.Application.Users.Requests;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Shared;

namespace Portfolio.Application.Users;

/// <summary>
/// Service implementation for User-related operations
/// </summary>
/// <param name="userRepository"></param>
public class UserService(IUserRepository userRepository) : IUserService
{
	private readonly IUserRepository _userRepository = userRepository;

	public async Task<Result<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByIdAsync(id, cancellationToken);

		return user != null ? Result<UserDto>.Success(new UserDto
		{
			Id = user.Id,
			Username = user.Name.Value,
			Email = user.Email.Value
		}) : Result<UserDto>.Failure(new Error("USER_NOT_FOUND", "User not found."));
	}

	public async Task<Result<UserDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByNameAsync(name, cancellationToken);
		return user != null ? Result<UserDto>.Success(new UserDto
		{
			Id = user.Id,
			Username = user.Name.Value,
			Email = user.Email.Value
		}) : Result<UserDto>.Failure(new Error("USER_NOT_FOUND", "User not found."));
	}

	public async Task<Result<IEnumerable<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		var users = await _userRepository.GetAllAsync(cancellationToken);

		if (!users.Any())
		{
			return Result<IEnumerable<UserDto>>.Failure(new Error("NO_USERS_FOUND", "No users found."));
		}

		return Result<IEnumerable<UserDto>>.Success(users.Select(user => new UserDto
		{
			Id = user.Id,
			Username = user.Name.Value,
			Email = user.Email.Value
		}));
	}

	public async Task<Result<UserDto>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByIdAsync(id, cancellationToken);
		if (user == null)
		{
			return Result<UserDto>.Failure(new Error("USER_NOT_FOUND", "User not found."));
		}

		await _userRepository.DeleteAsync(user, cancellationToken);
		return Result<UserDto>.Success(new UserDto
		{
			Id = user.Id,
			Username = user.Name.Value,
			Email = user.Email.Value
		});
	}

}