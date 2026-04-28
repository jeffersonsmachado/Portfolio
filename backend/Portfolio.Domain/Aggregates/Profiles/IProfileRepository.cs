namespace Portfolio.Domain.Aggregates.Profiles;

/// <summary>
/// Repository interface for Profile aggregate
/// </summary>
public interface IProfileRepository
{
	Task<Profile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IEnumerable<Profile>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<Profile?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
	Task AddAsync(Profile profile, CancellationToken cancellationToken = default);
	Task UpdateAsync(Profile profile, CancellationToken cancellationToken = default);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
	Task<Profile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
