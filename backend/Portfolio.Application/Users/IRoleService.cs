using Portfolio.Application.Users.Dtos;
using Portfolio.Domain.Shared;

namespace Portfolio.Application.Users;

public interface IRoleService
{
	Task<Result<RoleDto>> CreateRoleAsync(string roleName, CancellationToken cancellationToken = default);
	Task<Result<RoleDto>> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<Result<RoleDto>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);
	Task<Result<IEnumerable<RoleDto>>> GetAllRolesAsync(CancellationToken cancellationToken = default);
	Task<Result<RoleDto>> DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default);
	Task<Result<RoleDto>> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
	Task<Result<RoleDto>> SetRolePermissionsAsync(Guid roleId, IEnumerable<Guid> permissionsId, CancellationToken cancellationToken = default);
}
