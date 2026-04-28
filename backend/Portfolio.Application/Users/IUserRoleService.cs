using Portfolio.Domain.Shared;

namespace Portfolio.Application.Users;

/// <summary>
/// Service interface for User Role-related operations
/// </summary>
/// 
/// <remarks>
/// This service handles operations related to assigning roles to users. It is separate from IUserService to
/// maintain a clear separation of concerns between user management and role management logic.
/// </remarks>
public interface IUserRoleService
{
	Task<Result<RoleDto>> AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

	Task<Result<RoleDto>> RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<RoleDto>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<UserDto>>> GetUsersInRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
	Task<Result<IEnumerable<UserWithRolesDto>>> GetAllUsersWithRolesAsync(CancellationToken cancellationToken = default);

	Task<Result<IEnumerable<RoleDto>>> SetUserRolesAsync(Guid userId, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);
}