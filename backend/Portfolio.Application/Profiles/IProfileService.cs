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
	Task<Result<ProfileDto>> UpdateAsync(Guid profileId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
    Task<Result<SkillDto>> AddSkillAsync(Guid profileId, AddSkillRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveSkillAsync(Guid profileId, Guid skillId, CancellationToken cancellationToken = default);
    Task<Result<ExperienceDto>> AddExperienceAsync(Guid profileId, AddExperienceRequest request, CancellationToken cancellationToken = default);
    Task<Result<ExperienceDto>> UpdateExperienceAsync(Guid profileId, Guid experienceId, UpdateExperienceRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveExperienceAsync(Guid profileId, Guid experienceId, CancellationToken cancellationToken = default);
    Task<Result<EducationDto>> AddEducationAsync(Guid profileId, AddEducationRequest request, CancellationToken cancellationToken = default);
    Task<Result<EducationDto>> UpdateEducationAsync(Guid profileId, Guid educationId, UpdateEducationRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveEducationAsync(Guid profileId, Guid educationId, CancellationToken cancellationToken = default);
}
