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
	private static ProfileDto MapToDto(Profile profile)
	{
		return new ProfileDto
		{
			Id = profile.Id,
			Name = profile.Name,
			UserName = profile.User.Name
		};
	}
}
