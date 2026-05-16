using Portfolio.Domain.Shared;

namespace Portfolio.Application.Users;

public interface IPermissionService
{
	Task<Result<PermissionDto>> CreatePermissionAsync(string permissionName, CancellationToken cancellationToken = default);
	Task<Result<PermissionDto>> GetPermissionByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<Result<PermissionDto>> GetPermissionByNameAsync(string permissionName, CancellationToken cancellationToken = default);
	Task<Result<IEnumerable<PermissionDto>>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);
	Task<Result<PermissionDto>> DeletePermissionAsync(Guid id, CancellationToken cancellationToken = default);
}
