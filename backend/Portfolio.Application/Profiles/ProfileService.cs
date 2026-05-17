using Portfolio.Domain.Aggregates.Profiles;
using Portfolio.Domain.Aggregates.Users;
using Portfolio.Domain.Shared;

namespace Portfolio.Application.Profiles;

/// <summary>
/// Service for managing Profile-related business logic
/// </summary>
public class ProfileService(IProfileRepository profileRepository, IUserRepository userRepository) : IProfileService
{
	private readonly IProfileRepository _profileRepository = profileRepository;
	private readonly IUserRepository _userRepository = userRepository;

	public async Task<Result<ProfileDto>> CreateAsync(CreateProfileRequest request, CancellationToken cancellationToken = default)
	{
		// Check if username already exists
		var existingProfile = await _profileRepository.GetByNameAsync(request.Name, cancellationToken);

		if (existingProfile != null)
		{
			return Result<ProfileDto>.Failure(new Error("USERNAME_ALREADY_EXISTS", $"Profile with username '{request.Name}' already exists"));
		}

		// Check if associated user exists
		var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
		if (user == null)
		{
			return Result<ProfileDto>.Failure(new Error("USER_NOT_FOUND", $"User with ID '{request.UserId}' not found"));
		}

		// Create profile using domain factory method
		var profile = Profile.Create(request.Name, user);
		// Save to repository
		await _profileRepository.AddAsync(profile, cancellationToken);

		// Map to DTO
		return Result<ProfileDto>.Success(MapToDto(profile));
	}

	public async Task<Result<ProfileDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var profile = await _profileRepository.GetByIdAsync(id, cancellationToken);
		return profile != null ? Result<ProfileDto>.Success(MapToDto(profile)) : Result<ProfileDto>.Failure(new Error("PROFILE_NOT_FOUND", $"Profile with ID '{id}' not found"));
	}

	public async Task<Result<ProfileDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		var profile = await _profileRepository.GetByNameAsync(name, cancellationToken);
		return profile != null ? Result<ProfileDto>.Success(MapToDto(profile)) : Result<ProfileDto>.Failure(new Error("PROFILE_NOT_FOUND", $"Profile with name '{name}' not found"));
	}

	public async Task<Result<IEnumerable<ProfileDto>>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		var profiles = await _profileRepository.GetAllAsync(cancellationToken);
		if (!profiles.Any())
		{
			return Result<IEnumerable<ProfileDto>>.Failure(new Error("NO_PROFILES_FOUND", "No profiles found."));
		}
		return Result<IEnumerable<ProfileDto>>.Success(profiles.Select(MapToDto));
	}

	public async Task<Result<ProfileDto>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var profile = await _profileRepository.GetByIdAsync(id, cancellationToken);
		if (profile == null)
		{
			return Result<ProfileDto>.Failure(new Error("PROFILE_NOT_FOUND", $"Profile with ID '{id}' not found"));
		}

		await _profileRepository.DeleteAsync(profile.Id, cancellationToken);
		return Result<ProfileDto>.Success(MapToDto(profile));
	}

	public async Task<Result<ProfileDto>> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		var profile = await _profileRepository.GetByUserIdAsync(userId, cancellationToken);
		return profile != null ? Result<ProfileDto>.Success(MapToDto(profile)) : Result<ProfileDto>.Failure(new Error("PROFILE_NOT_FOUND", $"Profile for User ID '{userId}' not found"));
	}
	
	public async Task<Result<ProfileDto>> UpdateAsync(Guid profileId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
	{
		var profile = await _profileRepository.GetByIdAsync(profileId, cancellationToken);

		if (profile == null)
		{
			return Result<ProfileDto>.Failure(new Error("PROFILE_NOT_FOUND", $"Profile with ID '{profileId}' not found"));
		}
		
		profile.UpdateProfile(request.Name, request.Bio, request.BioTitle, request.AvatarUrl);

		await _profileRepository.UpdateAsync(profile, cancellationToken);
		
		return Result<ProfileDto>.Success(MapToDto(profile));
	}

	public async Task<Result<SkillDto>> AddSkillAsync(Guid profileId, AddSkillRequest request, CancellationToken cancellationToken = default)
	{
		var profile = await _profileRepository.GetByIdAsync(profileId, cancellationToken);

		if (profile == null)
		{
			return Result<SkillDto>.Failure(new Error("PROFILE_NOT_FOUND", $"Profile with ID '{profileId}' not found"));
		}

		var skill = profile.AddSkill(request.Name);

		if (skill == null)
		{
			return Result<SkillDto>.Failure(new Error("COULD_NOT_ADD_SKILL", $"Skill '{request.Name}' couldn't be added to profile with ID '{profileId}'"));
		}

		await _profileRepository.UpdateAsync(profile, cancellationToken);

		return Result<SkillDto>.Success(MapSkillToDto(skill));
	}

    public async Task<Result<bool>> RemoveSkillAsync(Guid profileId, Guid skillId, CancellationToken cancellationToken = default)
	{
		var profile = await _profileRepository.GetByIdAsync(profileId, cancellationToken);

		if (profile == null)
		{
			return Result<bool>.Failure(new Error("PROFILE_NOT_FOUND", $"Profile with ID '{profileId}' not found"));
		}

		profile.RemoveSkill(skillId);

		await _profileRepository.UpdateAsync(profile, cancellationToken);
		
		return Result<bool>.Success(true);
	}

    public async Task<Result<ExperienceDto>> AddExperienceAsync(Guid profileId, AddExperienceRequest request, CancellationToken cancellationToken = default)
	{
		var profile = await _profileRepository.GetByIdAsync(profileId, cancellationToken);

		if (profile == null)
		{
			return Result<ExperienceDto>.Failure(new Error("PROFILE_NOT_FOUND", $"Profile with ID '{profileId}' not found"));
		}

		var experience = profile.AddExperience(request.Company, request.Role, request.StartMonth, request.StartYear, request.EndMonth, request.EndYear, request.Current, request.Description);

		if (experience == null)
		{
			return Result<ExperienceDto>.Failure(new Error("COULD_NOT_ADD_EXPERIENCE", $"Experience for company '{request.Company}' couldn't be added to profile with ID '{profileId}'"));
		}

		await _profileRepository.UpdateAsync(profile, cancellationToken);

		return Result<ExperienceDto>.Success(MapExperienceToDto(experience));
	}

    public async Task<Result<ExperienceDto>> UpdateExperienceAsync(Guid profileId, Guid experienceId, UpdateExperienceRequest request, CancellationToken cancellationToken = default)
	{
		var profile = await _profileRepository.GetByIdAsync(profileId, cancellationToken);

		if (profile == null)
		{
			return Result<ExperienceDto>.Failure(new Error("PROFILE_NOT_FOUND", $"Profile with ID '{profileId}' not found"));
		}

		var experience = profile.UpdateExperience(experienceId, request.Company, request.Role, request.StartMonth, request.StartYear, request.EndMonth, request.EndYear, request.Current, request.Description);

		if (experience == null)
		{
			return Result<ExperienceDto>.Failure(new Error("COULD_NOT_UPDATE_EXPERIENCE", $"Experience with ID '{experienceId}' couldn't be updated for profile with ID '{profileId}'"));
		}

		await _profileRepository.UpdateAsync(profile, cancellationToken);

		return Result<ExperienceDto>.Success(MapExperienceToDto(experience));
	}

    public async Task<Result<bool>> RemoveExperienceAsync(Guid profileId, Guid experienceId, CancellationToken cancellationToken = default)
	{
		var profile = await _profileRepository.GetByIdAsync(profileId, cancellationToken);

		if (profile == null)
		{
			return Result<bool>.Failure(new Error("PROFILE_NOT_FOUND", $"Profile with ID '{profileId}' not found"));
		}

		profile.RemoveExperience(experienceId);

		await _profileRepository.UpdateAsync(profile, cancellationToken);
		
		return Result<bool>.Success(true);

	}

    public async Task<Result<EducationDto>> AddEducationAsync(Guid profileId, AddEducationRequest request, CancellationToken cancellationToken = default)
	{
		var profile = await _profileRepository.GetByIdAsync(profileId, cancellationToken);

		if (profile == null)
		{
			return Result<EducationDto>.Failure(new Error("PROFILE_NOT_FOUND", $"Profile with ID '{profileId}' not found"));
		}

		var education = profile.AddEducation(request.Institution, request.Degree, request.StartMonth, request.StartYear, request.EndMonth, request.EndYear);

		if (education == null)
		{
			return Result<EducationDto>.Failure(new Error("COULD_NOT_ADD_EDUCATION", $"Education at institution '{request.Institution}' couldn't be added to profile with ID '{profileId}'"));
		}

		await _profileRepository.UpdateAsync(profile, cancellationToken);

		return Result<EducationDto>.Success(MapEducationToDto(education));
	}

    public async Task<Result<EducationDto>> UpdateEducationAsync(Guid profileId, Guid educationId, UpdateEducationRequest request, CancellationToken cancellationToken = default)
	{
		var profile = await _profileRepository.GetByIdAsync(profileId, cancellationToken);

		if (profile == null)
		{
			return Result<EducationDto>.Failure(new Error("PROFILE_NOT_FOUND", $"Profile with ID '{profileId}' not found"));
		}

		var education = profile.UpdateEducation(educationId, request.Institution, request.Degree, request.StartMonth, request.StartYear, request.EndMonth, request.EndYear);

		if (education == null)
		{
			return Result<EducationDto>.Failure(new Error("COULD_NOT_UPDATE_EDUCATION", $"Education with ID '{educationId}' couldn't be updated for profile with ID '{profileId}'"));
		}

		await _profileRepository.UpdateAsync(profile, cancellationToken);

		return Result<EducationDto>.Success(MapEducationToDto(education));
	}

    public async Task<Result<bool>> RemoveEducationAsync(Guid profileId, Guid educationId, CancellationToken cancellationToken = default)
	{
		var profile = await _profileRepository.GetByIdAsync(profileId, cancellationToken);

		if (profile == null)
		{
			return Result<bool>.Failure(new Error("PROFILE_NOT_FOUND", $"Profile with ID '{profileId}' not found"));
		}

		profile.RemoveEducation(educationId);

		await _profileRepository.UpdateAsync(profile, cancellationToken);
		
		return Result<bool>.Success(true);
	}

	
	private static ProfileDto MapToDto(Profile profile)
	{
		return new ProfileDto
		{
			Id = profile.Id,
			Name = profile.Name,
			UserName = profile.User?.Name.Value ?? string.Empty,
			Bio = profile.Bio,
			BioTitle = profile.BioTitle,
			AvatarUrl = profile.AvatarUrl,
			Skills = [.. profile.Skills.Select(MapSkillToDto)],
			Experiences = [.. profile.Experiences.Select(MapExperienceToDto)],
			Educations = [.. profile.Educations.Select(MapEducationToDto)]
        };
	}

	private static SkillDto MapSkillToDto(Skill skill)
	{
		return new SkillDto
		{
			Id = skill.Id,
			Name = skill.Name
		};
	}

	private static ExperienceDto MapExperienceToDto(Experience experience)
	{
		return new ExperienceDto
		{
			Id = experience.Id,
			Company = experience.Company,
			Role = experience.Role,
			StartMonth = experience.StartMonth,
			StartYear = experience.StartYear,
			EndMonth = experience.EndMonth,
			EndYear = experience.EndYear,
			Current = experience.Current,
			Description = experience.Description
		};
	}

	private static EducationDto MapEducationToDto(Education education)
	{
		return new EducationDto
		{
			Id = education.Id,
			Institution = education.Institution,
			Degree = education.Degree,
			StartMonth = education.StartMonth,
			StartYear = education.StartYear,
			EndMonth = education.EndMonth,
			EndYear = education.EndYear
		};
	}
}
