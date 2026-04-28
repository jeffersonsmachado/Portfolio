namespace Portfolio.Domain.Aggregates.Users;

/// <summary>
/// Repository interface for User aggregate
/// </summary>
public interface IUserRepository
{
	Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task AddAsync(User user, CancellationToken cancellationToken = default);
	Task UpdateAsync(User user, CancellationToken cancellationToken = default);
	Task DeleteAsync(User user, CancellationToken cancellationToken = default);
	Task<IEnumerable<User>> GetUsersInRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
	Task<IEnumerable<User>> GetAllWithRolesAsync(CancellationToken cancellationToken = default);
	Task<User?> GetByIdWithRolesAsync(Guid id, CancellationToken cancellationToken = default);
}