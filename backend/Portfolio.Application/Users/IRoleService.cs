using Portfolio.Domain.Shared;

namespace Portfolio.Application.Users;

public interface IRoleService
{
	public Task<Result<RoleDto>> CreateRoleAsync(string roleName, CancellationToken cancellationToken = default);
	public Task<Result<RoleDto>> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
	public Task<Result<RoleDto>> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);
	public Task<Result<IEnumerable<RoleDto>>> GetAllRolesAsync(CancellationToken cancellationToken = default);
	public Task<Result<RoleDto>> DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default);
	public Task<Result<RoleDto>> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}
