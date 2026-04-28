using Portfolio.Domain.Shared;

namespace Portfolio.Application.Users;

public interface IPermissionService
{
	public Task<Result<PermissionDto>> CreatePermissionAsync(string permissionName, CancellationToken cancellationToken = default);
	public Task<Result<PermissionDto>> GetPermissionByIdAsync(Guid id, CancellationToken cancellationToken = default);
	public Task<Result<PermissionDto>> GetPermissionByNameAsync(string permissionName, CancellationToken cancellationToken = default);
	public Task<Result<IEnumerable<PermissionDto>>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);
	public Task<Result<PermissionDto>> DeletePermissionAsync(Guid id, CancellationToken cancellationToken = default);
}
