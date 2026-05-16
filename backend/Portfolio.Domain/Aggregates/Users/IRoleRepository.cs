using Portfolio.Domain.Aggregates.Users;

namespace Portfolio.Domain.Aggregates.Roles;

/// <summary>
/// Repository interface for Role aggregate
/// </summary>
public interface IRoleRepository
{
	Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
	Task AddAsync(Role role, CancellationToken cancellationToken = default);
	Task UpdateAsync(Role role, CancellationToken cancellationToken = default);
	Task DeleteAsync(Role role, CancellationToken cancellationToken = default);
}