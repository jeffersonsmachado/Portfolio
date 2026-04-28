using Portfolio.Domain.Aggregates.Roles;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Shared;

namespace Portfolio.Application.Users;

public class RoleService(IRoleRepository roleRepository, IUserRepository userRepository) : IRoleService
{
	private readonly IRoleRepository _roleRepository = roleRepository;
	private readonly IUserRepository _userRepository = userRepository;

	public async Task<Result<RoleDto>> CreateRoleAsync(string roleName, CancellationToken cancellationToken = default)
	{
		var existing = await _roleRepository.GetByNameAsync(roleName, cancellationToken);
		if (existing is not null)
			return Result<RoleDto>.Failure(new Error("ROLE_ALREADY_EXISTS", $"Role '{roleName}' already exists."));

		var role = Role.Create(roleName, [], []);
		await _roleRepository.AddAsync(role, cancellationToken);

		return Result<RoleDto>.Success(new RoleDto { Id = role.Id, Name = role.Name });
	}

	public async Task<Result<RoleDto>> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
		if (role is null)
			return Result<RoleDto>.Failure(new Error("ROLE_NOT_FOUND", "Role not found."));

		return Result<RoleDto>.Success(new RoleDto { Id = role.Id, Name = role.Name });
	}

	public async Task<Result<RoleDto>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
	{
		var role = await _roleRepository.GetByNameAsync(roleName, cancellationToken);
		if (role is null)
			return Result<RoleDto>.Failure(new Error("ROLE_NOT_FOUND", "Role not found."));

		return Result<RoleDto>.Success(new RoleDto { Id = role.Id, Name = role.Name });
	}

	public async Task<Result<IEnumerable<RoleDto>>> GetAllRolesAsync(CancellationToken cancellationToken = default)
	{
		var roles = await _roleRepository.GetAllAsync(cancellationToken);
		var dtos = roles.Select(r => new RoleDto { Id = r.Id, Name = r.Name });
		return Result<IEnumerable<RoleDto>>.Success(dtos);
	}

	public async Task<Result<RoleDto>> DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
		if (role is null)
			return Result<RoleDto>.Failure(new Error("ROLE_NOT_FOUND", "Role not found."));

		await _roleRepository.DeleteAsync(role, cancellationToken);
		return Result<RoleDto>.Success(new RoleDto { Id = role.Id, Name = role.Name });
	}

	public async Task<Result<RoleDto>> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
	{
		var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
		if (user is null)
			return Result<RoleDto>.Failure(new Error("USER_NOT_FOUND", "User not found."));

		var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
		if (role is null)
			return Result<RoleDto>.Failure(new Error("ROLE_NOT_FOUND", "Role not found."));

		user.AssignRole(role);
		await _userRepository.UpdateAsync(user, cancellationToken);

		return Result<RoleDto>.Success(new RoleDto { Id = role.Id, Name = role.Name });
	}
}

