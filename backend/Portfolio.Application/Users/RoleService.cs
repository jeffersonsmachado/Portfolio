using Portfolio.Application.Users.Dtos;
using Portfolio.Domain.Aggregates.Permissions;
using Portfolio.Domain.Aggregates.Roles;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Shared;

namespace Portfolio.Application.Users;

public class RoleService(IRoleRepository roleRepository, IUserRepository userRepository, IPermissionRepository permissionRepository) : IRoleService
{
	private readonly IRoleRepository _roleRepository = roleRepository;
	private readonly IUserRepository _userRepository = userRepository;
	private readonly IPermissionRepository _permissionRepository = permissionRepository;

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
		var dtos = roles.Select(r => new RoleDto
		{
			Id = r.Id,
			Name = r.Name,
			Permissions = r.Permissions.Select(p => new PermissionDto { Id = p.Id, Name = p.Name }).ToList()
		});
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

	public async Task<Result<RoleDto>> SetRolePermissionsAsync(Guid roleId, IEnumerable<Guid> permissionsId, CancellationToken cancellationToken = default)
	{
		var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
		
		if (role is null)
		{
			return Result<RoleDto>.Failure(new Error("ROLE_NOT_FOUND", "Role not found."));
		}

		var permissions = new List<Permission>();

		foreach (var permitionId in permissionsId)
		{
			var perm = await _permissionRepository.GetByIdAsync(permitionId, cancellationToken);
			
			if (perm is null)
			{
				return Result<RoleDto>.Failure(new Error("PERMISSION_NOT_FOUND", $"Permission with ID '{permitionId}' not found."));
			}

			permissions.Add(perm);
		}

		role.UpdatePermissions(permissions);
		await _roleRepository.UpdateAsync(role, cancellationToken);

		return Result<RoleDto>.Success(new RoleDto
		{
			Id = role.Id,
			Name = role.Name,
			Permissions = role.Permissions.Select(p => new PermissionDto { Id = p.Id, Name = p.Name }).ToList()
		});
	}
}

