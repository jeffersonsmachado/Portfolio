using Portfolio.Domain.Aggregates.Users;

namespace Portfolio.Domain.Aggregates.Permissions;

/// <summary>
/// Repository interface for Permission aggregate
/// </summary>
public interface IPermissionRepository
{
	Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
	Task<Permission?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task AddAsync(Permission permission, CancellationToken cancellationToken = default);
	Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default);
	Task DeleteAsync(Permission permission, CancellationToken cancellationToken = default);
}