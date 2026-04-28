using Portfolio.Domain.Shared;

namespace Portfolio.Application.Profiles;

/// <summary>
/// Service interface for Profile-related operations
/// </summary>
public interface IProfileService
{
	Task<Result<ProfileDto>> CreateAsync(CreateProfileRequest request, CancellationToken cancellationToken = default);
	Task<Result<ProfileDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<Result<ProfileDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
	Task<Result<IEnumerable<ProfileDto>>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<Result<ProfileDto>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
	Task<Result<ProfileDto>> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
