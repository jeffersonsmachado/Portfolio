using Portfolio.Domain.Aggregates.Roles;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Shared;

namespace Portfolio.Application.Users;

public class UserRoleService(IUserRepository userRepository, IRoleRepository roleRepository) : IUserRoleService
{
	private readonly IUserRepository _userRepository = userRepository;
	private readonly IRoleRepository _roleRepository = roleRepository;


	public async Task<Result<RoleDto>> AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByIdWithRolesAsync(userId, cancellationToken);

		if (user == null)
		{
			return Result<RoleDto>.Failure(new Error("USER_NOT_FOUND", "User not found."));
		}

		var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);

		if (role == null)
		{
			return Result<RoleDto>.Failure(new Error("ROLE_NOT_FOUND", "Role not found."));
		}

		user.AssignRole(role);

		await _userRepository.UpdateAsync(user, cancellationToken);

		var assignedRole = user.Roles.FirstOrDefault(r => r.Id == roleId);
		if (assignedRole == null)
		{
			return Result<RoleDto>.Failure(new Error("ROLE_ASSIGNMENT_FAILED", "Failed to assign role to user."));
		}

		return Result<RoleDto>.Success(new RoleDto
		{
			Id = assignedRole.Id,
			Name = assignedRole.Name
		});
	}

	public async Task<Result<RoleDto>> RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByIdWithRolesAsync(userId, cancellationToken);

		if (user == null)
		{
			return Result<RoleDto>.Failure(new Error("USER_NOT_FOUND", "User not found."));
		}

		var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);

		if (role == null)
		{
			return Result<RoleDto>.Failure(new Error("ROLE_NOT_FOUND", "Role not found."));
		}

		user.RemoveRole(role);

		await _userRepository.UpdateAsync(user, cancellationToken);

		return Result<RoleDto>.Success(new RoleDto
		{
			Id = role.Id,
			Name = role.Name
		});

	}

	public async Task<Result<IEnumerable<RoleDto>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByIdWithRolesAsync(userId, cancellationToken);

		if (user == null)
		{
			return Result<IEnumerable<RoleDto>>.Failure(new Error("USER_NOT_FOUND", "User not found."));
		}

		var roles = user.Roles.Select(r => new RoleDto
		{
			Id = r.Id,
			Name = r.Name
		});

		return Result<IEnumerable<RoleDto>>.Success(roles);
	}

	public async Task<Result<IEnumerable<UserDto>>> GetUsersInRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
	{
		var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);

		if (role == null)
		{
			return Result<IEnumerable<UserDto>>.Failure(new Error("ROLE_NOT_FOUND", "Role not found."));
		}

		var usersInRole = await _userRepository.GetUsersInRoleAsync(roleId, cancellationToken);

		var userDtos = usersInRole.Select(u => new UserDto
		{
			Id = u.Id,
			Username = u.Name.Value,
			Email = u.Email.Value
		});

		return Result<IEnumerable<UserDto>>.Success(userDtos);
	}

	public async Task<Result<IEnumerable<UserWithRolesDto>>> GetAllUsersWithRolesAsync(CancellationToken cancellationToken = default)
	{
		var users = await _userRepository.GetAllWithRolesAsync(cancellationToken);

		var userDtos = users.Select(u => new UserWithRolesDto
		{
			Id = u.Id,
			Username = u.Name.Value,
			Email = u.Email.Value,
			Roles = [.. u.Roles.Select(r => new RoleDto
			{
				Id = r.Id,
				Name = r.Name
			})]
		});

		return Result<IEnumerable<UserWithRolesDto>>.Success(userDtos);
	}

	public async Task<Result<IEnumerable<RoleDto>>> SetUserRolesAsync(Guid userId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByIdWithRolesAsync(userId, cancellationToken);
		if (user == null)
			return Result<IEnumerable<RoleDto>>.Failure(new Error("USER_NOT_FOUND", "User not found."));

		var targetIds = roleIds.ToHashSet();

		// Remove roles no longer in the new set
		var toRemove = user.Roles.Where(r => !targetIds.Contains(r.Id)).ToList();
		foreach (var role in toRemove)
			user.RemoveRole(role);

		// Add roles not yet assigned
		var currentIds = user.Roles.Select(r => r.Id).ToHashSet();
		foreach (var roleId in targetIds.Where(id => !currentIds.Contains(id)))
		{
			var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
			if (role == null)
				return Result<IEnumerable<RoleDto>>.Failure(new Error("ROLE_NOT_FOUND", $"Role {roleId} not found."));
			user.AssignRole(role);
		}

		await _userRepository.UpdateAsync(user, cancellationToken);

		return Result<IEnumerable<RoleDto>>.Success(user.Roles.Select(r => new RoleDto { Id = r.Id, Name = r.Name }));
	}
}